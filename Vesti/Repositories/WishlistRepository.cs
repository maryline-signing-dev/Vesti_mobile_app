using SQLite;
using Vesti.Data;
using Vesti.Models;

namespace Vesti.Repositories;

public class WishlistRepository
{
    private readonly SQLiteAsyncConnection _db;

    public WishlistRepository(VestiDatabase database)
        => _db = database.Connection;

    /// <summary>Souhaits non acquis, plus récents d'abord, filtrés par terme si fourni.</summary>
    public async Task<List<WishlistItem>> GetActiveAsync(string? term)
    {
        var query = _db.Table<WishlistItem>().Where(w => w.AcquiredAt == null);

        if (!string.IsNullOrWhiteSpace(term))
        {
            var t = term.Trim();
            query = query.Where(w => w.Name.Contains(t));
        }

        return await query.OrderByDescending(w => w.CreatedAt).ToListAsync();
    }

    public async Task<WishlistItem?> GetByIdAsync(int id)
        => await _db.FindAsync<WishlistItem>(id);

    /// <summary>Nombre de souhaits actifs (non acquis) — pour l'accueil et le dressing.</summary>
    public async Task<int> CountActiveAsync()
        => await _db.Table<WishlistItem>().Where(w => w.AcquiredAt == null).CountAsync();

    public async Task InsertAsync(WishlistItem item)
    {
        item.CreatedAt = item.UpdatedAt = DateTime.UtcNow;
        await _db.InsertAsync(item);
    }

    public async Task UpdateAsync(WishlistItem item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        await _db.UpdateAsync(item);
    }

    public async Task DeleteAsync(int id)
        => await _db.DeleteAsync<WishlistItem>(id);
}