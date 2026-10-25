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
    private readonly OutfitService _outfitService;
    private readonly IImageStore _imageStore;

    public ClothingService(ClothingRepository repository, IImageStore imageStore)
    {
        _repository = repository;
        _imageStore = imageStore;
    }

    // ── Lecture  ───────────────────────────────────────────────

    public async Task<List<ClothingItem>> GetItemsAsync(string? term, string? category)
    {
        var categoryFilter =
            string.IsNullOrWhiteSpace(category) || category == ClothingOptions.AllFilter
                ? null : category;

        return await _repository.QueryAsync(term, categoryFilter);
    }

    public async Task<ClothingItem?> GetByIdAsync(int id)
        => await _repository.GetByIdAsync(id);

    public async Task<int> CountAsync()
        => await _repository.CountAsync();

    public async Task<List<ClothingItem>> GetRecentAsync(int count = 4)
        => await _repository.GetRecentAsync(count);

    /// <summary>Pièces marquées favorites.</summary>
    public async Task<List<ClothingItem>> GetFavoritesAsync()
        => (await _repository.QueryAsync(null, null))
            .Where(i => i.IsFavorite)
            .ToList();


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


    /// Met à jour une pièce.

    public async Task<string?> UpdateAsync(ClothingItem item)
    {
        Normalize(item);
        var error = ValidateItem(item);
        if (error is not null) return error;
        await _repository.UpdateAsync(item);
        return null;
    }

    /// <summary>
    /// On retire la pièce de toutes ses tenues AVANT de la supprimer.
    /// Les tenues devenues vides sont supprimées automatiquement.
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        await _outfitService.OnClothingItemDeletedAsync(id);
        await _repository.DeleteAsync(id);
    }


    /// <summary>Bascule l'état favori d'une pièce et retourne le nouvel état.</summary>
    public async Task<bool> ToggleFavoriteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null) return false;

        item.IsFavorite = !item.IsFavorite;
        await _repository.UpdateAsync(item);
        return item.IsFavorite;
    }

    // ── Règles internes  ─────────────────────────────────────

    private static void Normalize(ClothingItem item)
    {
        item.Name = item.Name?.Trim() ?? string.Empty;
        item.Category = item.Category?.Trim() ?? string.Empty;
        item.Color = EmptyToNull(item.Color);
        item.Description = EmptyToNull(item.Description);
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
