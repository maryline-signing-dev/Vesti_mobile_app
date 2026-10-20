using Vesti.Models;
using Vesti.Repositories;

namespace Vesti.Services;

/// <summary>
/// Logique métier du dressing.
/// - Résultat d'une création de pièce : soit la pièce enregistrée,
/// - soit un message d'erreur destiné à l'utilisateur.
/// - Reçoit IImageStore en paramètre (injection de dépendances).
/// - DeleteAsync supprime maintenant le fichier image associé AVANT
///   de supprimer l'enregistrement en base.
/// - UpdateAsync : si une nouvelle photo remplace l'ancienne,
/// -  validation, nettoyage des saisies,
/// recherche + filtre combinés. Les ViewModels appellent ce service,
/// </summary>
public record AddItemResult(ClothingItem? Item, string? Error);

public class ClothingService
{
    private readonly ClothingRepository _repository;
    private readonly IImageStore _imageStore;

    public ClothingService(ClothingRepository repository, IImageStore imageStore)
    {
        _repository = repository;
        _imageStore = imageStore;
    }

    // ── Lecture (inchangé) ───────────────────────────────────────────────

    public async Task<List<ClothingItem>> GetItemsAsync(string? term, string? category)
    {
        var categoryFilter =
            string.IsNullOrWhiteSpace(category) || category == ClothingOptions.AllFilter
                ? null
                : category;

        return await _repository.QueryAsync(term, categoryFilter);
    }

    public async Task<ClothingItem?> GetByIdAsync(int id)
        => await _repository.GetByIdAsync(id);

    public async Task<int> CountAsync()
        => await _repository.CountAsync();

    public async Task<List<ClothingItem>> GetRecentAsync(int count = 4)
        => await _repository.GetRecentAsync(count);

    // ── Écriture ─────────────────────────────────────────────────────────


    public async Task<AddItemResult> AddAsync(ClothingItem item)
    {
        Normalize(item);

        var error = ValidateItem(item);
        if (error is not null)
            return new AddItemResult(null, error);

        await _repository.InsertAsync(item);
        return new AddItemResult(item, null);
    }




    /// <summary>
    /// Met à jour une pièce.
    /// si l'ImagePath a changé, l'ancien fichier est supprimé.
    /// </summary>
    public async Task<string?> UpdateAsync(ClothingItem item)
    {
        Normalize(item);

        var error = ValidateItem(item);
        if (error is not null) return error;

        // Récupérer l'ancienne image pour la supprimer si elle change
        var existing = await _repository.GetByIdAsync(item.Id);
        var oldPath = existing?.ImagePath;
        var newPath = item.ImagePath;

        await _repository.UpdateAsync(item);

        // Supprimer l'ancien fichier seulement s'il est différent et non null
        if (oldPath is not null && oldPath != newPath)
            await _imageStore.DeleteAsync(oldPath);

        return null;
    }

    /// <summary>
    /// I3 : supprime d'abord le fichier photo, puis l'enregistrement.
    /// L'ordre est important : si la suppression du fichier échoue,
    /// on ne supprime pas la fiche (cohérence).
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is not null)
            await _imageStore.DeleteAsync(item.ImagePath);

        await _repository.DeleteAsync(id);
    }

    // ── Règles internes (inchangées) ─────────────────────────────────────

    private static void Normalize(ClothingItem item)
    {
        item.Name = item.Name?.Trim() ?? string.Empty;
        item.Category = item.Category?.Trim() ?? string.Empty;
        item.Color = EmptyToNull(item.Color);
        item.Description = EmptyToNull(item.Description);
        item.Season = EmptyToNull(item.Season);
        item.Style = EmptyToNull(item.Style);
        item.Brand = EmptyToNull(item.Brand);
    }

    private static string? EmptyToNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? ValidateItem(ClothingItem item)
    {
        if (string.IsNullOrEmpty(item.Name))
            return "Le nom de la pièce est obligatoire.";

        if (item.Name.Length > 100)
            return "Le nom ne peut pas dépasser 100 caractères.";

        if (string.IsNullOrEmpty(item.Category))
            return "Choisissez une catégorie.";

        if (!ClothingOptions.Categories.Contains(item.Category))
            return "La catégorie choisie n'est pas valide.";

        return null;
    }
}
