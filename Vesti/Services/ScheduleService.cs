using Vesti.Models;
using Vesti.Repositories;

namespace Vesti.Services;

public record SaveScheduleResult(OutfitSchedule? Schedule, string? Error);

/// <summary>
/// Logique métier des planifications.
/// Charge toujours la tenue associée (OutfitSchedule.Outfit) pour l'affichage.
/// </summary>
public class ScheduleService
{
    private readonly OutfitScheduleRepository _repository;
    private readonly OutfitService _outfitService;

    public ScheduleService(OutfitScheduleRepository repository, OutfitService outfitService)
    {
        _repository = repository;
        _outfitService = outfitService;
    }

    // ── Lecture ──────────────────────────────────────────────────────────

    /// <summary>Planifications du mois (année/mois donnés), tenues chargées.</summary>
    public async Task<List<OutfitSchedule>> GetForMonthAsync(int year, int month)
    {
        var from = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
        var to = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString("yyyy-MM-dd");

        var schedules = await _repository.GetByDateRangeAsync(from, to);
        await LoadOutfitsAsync(schedules);
        return schedules;
    }

    /// <summary>Dates du mois qui ont au moins une planification (pour les points du calendrier).</summary>
    public async Task<HashSet<string>> GetScheduledDatesForMonthAsync(int year, int month)
    {
        var from = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
        var to = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString("yyyy-MM-dd");
        return await _repository.GetScheduledDatesAsync(from, to);
    }

    /// <summary>Planifications d'une journée précise, tenues chargées.</summary>
    public async Task<List<OutfitSchedule>> GetForDateAsync(DateTime date)
    {
        var schedules = await _repository.GetByDateAsync(date.ToString("yyyy-MM-dd"));
        await LoadOutfitsAsync(schedules);
        return schedules;
    }

    public async Task<OutfitSchedule?> GetByIdAsync(int id)
    {
        var schedule = await _repository.GetByIdAsync(id);
        if (schedule is null) return null;
        schedule.Outfit = await _outfitService.GetByIdAsync(schedule.OutfitId);
        return schedule;
    }

    public async Task<int> CountAsync()
        => await _repository.CountAsync();

    // ── Écriture ─────────────────────────────────────────────────────────

    public async Task<SaveScheduleResult> CreateAsync(
        int outfitId, DateTime date, TimeSpan? time, string? occasion)
    {
        var outfit = await _outfitService.GetByIdAsync(outfitId);
        if (outfit is null)
            return new SaveScheduleResult(null, "Cette tenue est introuvable.");

        var schedule = new OutfitSchedule
        {
            OutfitId = outfitId,
            ScheduledDate = date.ToString("yyyy-MM-dd"),
            ScheduledTime = time?.ToString(@"hh\:mm"),
            Occasion = string.IsNullOrWhiteSpace(occasion) ? null : occasion
        };

        await _repository.InsertAsync(schedule);
        schedule.Outfit = outfit;

        return new SaveScheduleResult(schedule, null);
    }

    public async Task<string?> UpdateAsync(
        int scheduleId, DateTime date, TimeSpan? time, string? occasion)
    {
        var schedule = await _repository.GetByIdAsync(scheduleId);
        if (schedule is null) return "Cette planification est introuvable.";

        schedule.ScheduledDate = date.ToString("yyyy-MM-dd");
        schedule.ScheduledTime = time?.ToString(@"hh\:mm");
        schedule.Occasion = string.IsNullOrWhiteSpace(occasion) ? null : occasion;

        await _repository.UpdateAsync(schedule);
        return null;
    }

    public async Task DeleteAsync(int id)
        => await _repository.DeleteAsync(id);

    /// <summary>Appelé quand une tenue est supprimée : retire ses planifications.</summary>
    public async Task OnOutfitDeletedAsync(int outfitId)
        => await _repository.DeleteByOutfitIdAsync(outfitId);

    // ── Interne ──────────────────────────────────────────────────────────

    private async Task LoadOutfitsAsync(List<OutfitSchedule> schedules)
    {
        foreach (var schedule in schedules)
            schedule.Outfit = await _outfitService.GetByIdAsync(schedule.OutfitId);
    }
}