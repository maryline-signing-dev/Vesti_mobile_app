using SQLite;
using Vesti.Data;
using Vesti.Models;

namespace Vesti.Repositories;

/// <summary>
/// Accès SQL pour les tenues et leurs liaisons avec les pièces.
/// Aucune règle métier ici : tout est dans OutfitService.
/// </summary>
public class OutfitRepository
{
    private readonly SQLiteAsyncConnection _db;

    public OutfitRepository(VestiDatabase database)
        => _db = database.Connection;

    // ── Outfits ──────────────────────────────────────────────────────────

    public async Task<List<Outfit>> GetAllAsync()
        => await _db.Table<Outfit>()
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

    public async Task<Outfit?> GetByIdAsync(int id)
        => await _db.FindAsync<Outfit>(id);

    public async Task<int> CountAsync()
        => await _db.Table<Outfit>().CountAsync();

    public async Task InsertAsync(Outfit outfit)
    {
        outfit.CreatedAt = outfit.UpdatedAt = DateTime.UtcNow;
        await _db.InsertAsync(outfit);
    }

    public async Task UpdateAsync(Outfit outfit)
    {
        outfit.UpdatedAt = DateTime.UtcNow;
        await _db.UpdateAsync(outfit);
    }

    public async Task DeleteAsync(int outfitId)
    {
        // Supprimer d'abord les liaisons, puis la tenue
        await _db.ExecuteAsync(
            "DELETE FROM OutfitItem WHERE OutfitId = ?", outfitId);
        await _db.DeleteAsync<Outfit>(outfitId);
    }

    // ── OutfitItems (liaisons) ────────────────────────────────────────────

    public async Task<List<OutfitItem>> GetItemLinksAsync(int outfitId)
        => await _db.Table<OutfitItem>()
                    .Where(l => l.OutfitId == outfitId)
                    .OrderBy(l => l.DisplayOrder)
                    .ToListAsync();

    public async Task InsertItemLinkAsync(OutfitItem link)
        => await _db.InsertAsync(link);

    public async Task DeleteItemLinksForOutfitAsync(int outfitId)
        => await _db.ExecuteAsync(
            "DELETE FROM OutfitItem WHERE OutfitId = ?", outfitId);

    /// <summary>
    /// Utilisé lors de la suppression d'une pièce (I4) :
    /// retire la pièce de toutes les tenues qui l'utilisent.
    /// Retourne les Ids des tenues affectées pour vérifier si elles
    /// sont devenues vides.
    /// </summary>
    public async Task<List<int>> DeleteItemLinksByClothingItemAsync(int clothingItemId)
    {
        var links = await _db.Table<OutfitItem>()
                             .Where(l => l.ClothingItemId == clothingItemId)
                             .ToListAsync();

        var affectedOutfitIds = links.Select(l => l.OutfitId).Distinct().ToList();

        foreach (var link in links)
            await _db.DeleteAsync(link);

        return affectedOutfitIds;
    }

    /// <summary>Vrai si la tenue n'a plus aucune pièce.</summary>
    public async Task<bool> IsOutfitEmptyAsync(int outfitId)
    {
        var count = await _db.Table<OutfitItem>()
                             .Where(l => l.OutfitId == outfitId)
                             .CountAsync();
        return count == 0;
    }

    /// <summary>
    /// Pièces d'une tenue, dans l'ordre d'affichage.
    /// Utilise un JOIN SQL via une requête brute.
    /// </summary>
    public async Task<List<ClothingItem>> GetClothingItemsForOutfitAsync(int outfitId)
    {
        return await _db.QueryAsync<ClothingItem>(
            @"SELECT c.*
              FROM ClothingItem c
              INNER JOIN OutfitItem oi ON oi.ClothingItemId = c.Id
              WHERE oi.OutfitId = ?
              ORDER BY oi.DisplayOrder",
            outfitId);
    }
}
