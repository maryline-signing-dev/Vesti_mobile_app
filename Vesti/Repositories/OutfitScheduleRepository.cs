using SQLite;
using Vesti.Data;
using Vesti.Models;

namespace Vesti.Repositories;

public class OutfitScheduleRepository
{
    private readonly SQLiteAsyncConnection _db;

    public OutfitScheduleRepository(VestiDatabase database)
        => _db = database.Connection;

    /// <summary>Toutes les planifications entre deux dates (incluses), format "yyyy-MM-dd".</summary>
    public async Task<List<OutfitSchedule>> GetByDateRangeAsync(string fromDate, string toDate)
        => await _db.Table<OutfitSchedule>()
                    .Where(s => s.ScheduledDate.CompareTo(fromDate) >= 0
                             && s.ScheduledDate.CompareTo(toDate) <= 0)
                    .ToListAsync();

    /// <summary>Planifications d'une journée précise, triées par heure.</summary>
    public async Task<List<OutfitSchedule>> GetByDateAsync(string date)
    {
        var results = await _db.Table<OutfitSchedule>()
                               .Where(s => s.ScheduledDate == date)
                               .ToListAsync();
        // Tri : sans heure en premier ("toute la journée"), puis par heure croissante
        return results
            .OrderBy(s => s.ScheduledTime is null ? 0 : 1)
            .ThenBy(s => s.ScheduledTime)
            .ToList();
    }

    public async Task<OutfitSchedule?> GetByIdAsync(int id)
        => await _db.FindAsync<OutfitSchedule>(id);

    public async Task<int> CountAsync()
        => await _db.Table<OutfitSchedule>().CountAsync();

    /// <summary>
    /// Toutes les dates (yyyy-MM-dd) ayant au moins une planification,
    /// dans l'intervalle donné — sert à placer les points indicateurs
    /// sur le calendrier sans charger chaque planification.
    /// </summary>
    public async Task<HashSet<string>> GetScheduledDatesAsync(string fromDate, string toDate)
    {
        var rows = await GetByDateRangeAsync(fromDate, toDate);
        return rows.Select(r => r.ScheduledDate).ToHashSet();
    }

    public async Task InsertAsync(OutfitSchedule schedule)
    {
        schedule.CreatedAt = schedule.UpdatedAt = DateTime.UtcNow;
        await _db.InsertAsync(schedule);
    }

    public async Task UpdateAsync(OutfitSchedule schedule)
    {
        schedule.UpdatedAt = DateTime.UtcNow;
        await _db.UpdateAsync(schedule);
    }

    public async Task DeleteAsync(int id)
        => await _db.DeleteAsync<OutfitSchedule>(id);

    /// <summary>Utilisé si une tenue est supprimée : retire ses planifications.</summary>
    public async Task DeleteByOutfitIdAsync(int outfitId)
        => await _db.ExecuteAsync(
            "DELETE FROM OutfitSchedule WHERE OutfitId = ?", outfitId);
}