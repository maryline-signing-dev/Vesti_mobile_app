using SQLite;
using Vesti.Data;
using Vesti.Models;

namespace Vesti.Repositories;

public class WearHistoryRepository
{
    private readonly SQLiteAsyncConnection _db;

    public WearHistoryRepository(VestiDatabase database)
        => _db = database.Connection;

    public async Task InsertAsync(WearHistory entry)
        => await _db.InsertAsync(entry);

    public async Task<int> CountAsync()
        => await _db.Table<WearHistory>().CountAsync();

    /// <summary>Tous les événements, utilisé pour calculer le palmarès (petit volume attendu).</summary>
    public async Task<List<WearHistory>> GetAllAsync()
        => await _db.Table<WearHistory>().ToListAsync();

    public async Task<List<WearHistory>> GetForOutfitAsync(int outfitId)
        => await _db.Table<WearHistory>()
                    .Where(w => w.OutfitId == outfitId)
                    .OrderByDescending(w => w.WornAt)
                    .ToListAsync();
}
