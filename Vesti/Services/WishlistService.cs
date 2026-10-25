using Vesti.Models;
using Vesti.Repositories;

namespace Vesti.Services;

public record SaveWishlistResult(WishlistItem? Item, string? Error);

/// <summary>
/// Logique métier de la wishlist.
///
/// TRANSFERT VERS LE DRESSING :
///   PrepareAcquisition() ne crée RIEN — elle retourne seulement un
///   ClothingItem pré-rempli (non enregistré) que l'utilisateur confirme
///   ou ajuste avant sauvegarde (voir AcquireViewModel). C'est la
///   confirmation explicite qui appelle ClothingService.AddAsync, puis
///   MarkAcquiredAsync pour sortir l'item de la liste active.
/// </summary>
public class WishlistService
{
    private readonly WishlistRepository _repository;

    public WishlistService(WishlistRepository repository)
    {
        _repository = repository;
    }

    // ── Lecture ──────────────────────────────────────────────────────────

    public async Task<List<WishlistItem>> GetActiveAsync(string? term = null)
        => await _repository.GetActiveAsync(term);

    public async Task<WishlistItem?> GetByIdAsync(int id)
        => await _repository.GetByIdAsync(id);

    public async Task<int> CountActiveAsync()
        => await _repository.CountActiveAsync();

    // ── Écriture ─────────────────────────────────────────────────────────

    public async Task<SaveWishlistResult> AddAsync(WishlistItem item)
    {
        Normalize(item);
        var error = Validate(item);
        if (error is not null) return new SaveWishlistResult(null, error);

        await _repository.InsertAsync(item);
        return new SaveWishlistResult(item, null);
    }

    public async Task<string?> UpdateAsync(WishlistItem item)
    {
        Normalize(item);
        var error = Validate(item);
        if (error is not null) return error;

        await _repository.UpdateAsync(item);
        return null;
    }

    public async Task DeleteAsync(int id)
        => await _repository.DeleteAsync(id);

    /// <summary>
    /// Pré-remplit une fiche ClothingItem à partir d'un souhait, SANS l'enregistrer.
    /// L'utilisateur confirme (ou ajuste) avant la sauvegarde réelle.
    /// </summary>
    public ClothingItem PrepareAcquisition(WishlistItem wish) => new()
    {
        Name = wish.Name,
        Category = wish.Category,
        Color = wish.Color,
        ImagePath = wish.ImagePath
    };

    /// <summary>
    /// Marque le souhait comme acquis APRÈS que la pièce a été réellement
    /// enregistrée dans le dressing (appelé depuis AcquireViewModel).
    /// Le souhait n'est pas supprimé : il reste consultable avec son statut.
    /// </summary>
    public async Task MarkAcquiredAsync(int wishlistItemId)
    {
        var item = await _repository.GetByIdAsync(wishlistItemId);
        if (item is null) return;

        item.AcquiredAt = DateTime.UtcNow;
        await _repository.UpdateAsync(item);
    }

    // ── Règles internes ──────────────────────────────────────────────────

    private static void Normalize(WishlistItem item)
    {
        item.Name = item.Name?.Trim() ?? string.Empty;
        item.Category = item.Category?.Trim() ?? string.Empty;
        item.Color = EmptyToNull(item.Color);
        item.Link = EmptyToNull(item.Link);
        item.Reference = EmptyToNull(item.Reference);
    }

    private static string? EmptyToNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? Validate(WishlistItem item)
    {
        if (string.IsNullOrEmpty(item.Name))
            return "Le nom de la pièce est obligatoire.";
        if (item.Name.Length > 100)
            return "Le nom ne peut pas dépasser 100 caractères.";
        if (string.IsNullOrEmpty(item.Category))
            return "Choisissez une catégorie.";
        if (!ClothingOptions.Categories.Contains(item.Category))
            return "La catégorie choisie n'est pas valide.";
        if (item.Price is < 0)
            return "Le prix ne peut pas être négatif.";
        return null;
    }
}