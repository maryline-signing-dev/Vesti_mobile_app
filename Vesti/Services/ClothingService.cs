using Vesti.Models;
using Vesti.Repositories;

namespace Vesti.Services;

/// <summary>
/// Résultat d'une création de pièce : soit la pièce enregistrée,
/// soit un message d'erreur destiné à l'utilisateur.
/// </summary>
public record AddItemResult(ClothingItem? Item, string? Error);

/// <summary>
/// Logique métier du dressing : validation, nettoyage des saisies,
/// recherche + filtre combinés. Les ViewModels appellent ce service,
/// jamais le repository.
/// </summary>
public class ClothingService
{
    private readonly ClothingRepository _repository;

    public ClothingService(ClothingRepository repository)
    {
        _repository = repository;
    }

    // ── Lecture ──────────────────────────────────────────────────────────

    /// <summary>
    /// Pièces correspondant à la recherche et à la catégorie.
    /// La catégorie "Tout" (ou vide) signifie : pas de filtre.
    /// </summary>
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

    /// <summary>Retourne null en cas de succès, sinon le message d'erreur.</summary>
    public async Task<string?> UpdateAsync(ClothingItem item)
    {
        Normalize(item);

        var error = ValidateItem(item);
        if (error is not null)
            return error;

        await _repository.UpdateAsync(item);
        return null;
    }

    /// <summary>
    /// Supprime une pièce.
    /// I3 : supprimera aussi le fichier photo associé.
    /// I4 : retirera aussi la pièce des tenues qui l'utilisent.
    /// </summary>
    public async Task DeleteAsync(int id)
        => await _repository.DeleteAsync(id);

    // ── Règles internes ──────────────────────────────────────────────────

    /// <summary>Supprime les espaces superflus ; une chaîne vide devient null.</summary>
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