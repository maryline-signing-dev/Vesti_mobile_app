using SQLite;
using Vesti.Data;
using Vesti.Models;

namespace Vesti.Repositories;

/// <summary>
/// Seule couche qui parle SQL pour les pièces du dressing.
/// Aucune règle métier ici : validation et règles de suppression
/// appartiennent à ClothingService.
/// </summary>
public class ClothingRepository
{
    private readonly SQLiteAsyncConnection _db;

    public ClothingRepository(VestiDatabase database)
    {
        _db = database.Connection;
    }

    /// <summary>
    /// Pièces filtrées (terme dans le nom ET/OU catégorie), plus récentes d'abord.
    /// Un filtre null ou vide est ignoré.
    /// </summary>
    public async Task<List<ClothingItem>> QueryAsync(string? term, string? category)
    {
        var query = _db.Table<ClothingItem>();

        if (!string.IsNullOrWhiteSpace(term))
        {
            var t = term.Trim();
            query = query.Where(c => c.Name.Contains(t));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var cat = category;
            query = query.Where(c => c.Category == cat);
        }

        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }

    public async Task<ClothingItem?> GetByIdAsync(int id)
        => await _db.FindAsync<ClothingItem>(id);

    public async Task<int> CountAsync()
        => await _db.Table<ClothingItem>().CountAsync();

    public async Task<List<ClothingItem>> GetRecentAsync(int count)
        => await _db.Table<ClothingItem>()
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(count)
                    .ToListAsync();

    /// <summary>Insère la pièce ; son Id est renseigné par SQLite.</summary>
    public async Task InsertAsync(ClothingItem item)
    {
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = item.CreatedAt;
        await _db.InsertAsync(item);
    }

    public async Task UpdateAsync(ClothingItem item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        await _db.UpdateAsync(item);
    }

    public async Task DeleteAsync(int id)
        => await _db.DeleteAsync<ClothingItem>(id);
}