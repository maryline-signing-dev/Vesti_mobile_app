using Vesti.Models;

namespace Vesti.Services;

/// <summary>Une ligne de la répartition par catégorie ("Bas : 2/6").</summary>
public record CategoryStat(string Category, int Count, int Total)
{
    public double Ratio => Total == 0 ? 0 : (double)Count / Total;
}

/// <summary>Une ligne du palmarès ("Tenue Casual, portée 4 fois").</summary>
public record OutfitWearStat(int OutfitId, string OutfitName, int WearCount);

/// <summary>
/// Calcule les statistiques du dressing.
///
/// IMPORTANT : aucune statistique n'est inventée. Les compteurs viennent
/// toujours des données réelles (ClothingItem, Outfit, WearHistory).
/// S'il n'y a aucune pièce, la répartition par catégorie est une liste vide
/// — jamais une valeur de démonstration.
/// </summary>
public class StatsService
{
    private readonly ClothingService _clothingService;
    private readonly OutfitService _outfitService;
    private readonly ScheduleService _scheduleService;
    private readonly WearHistoryService _wearHistoryService;

    public StatsService(ClothingService clothingService,
                        OutfitService outfitService,
                        ScheduleService scheduleService,
                        WearHistoryService wearHistoryService)
    {
        _clothingService = clothingService;
        _outfitService = outfitService;
        _scheduleService = scheduleService;
        _wearHistoryService = wearHistoryService;
    }

    /// <summary>Compteurs globaux (pièces, tenues, planifications).</summary>
    public async Task<(int Pieces, int Outfits, int Scheduled)> GetOverviewAsync()
    {
        var pieces = await _clothingService.CountAsync();
        var outfits = await _outfitService.CountAsync();
        var scheduled = await _scheduleService.CountAsync();
        return (pieces, outfits, scheduled);
    }

    /// <summary>
    /// Répartition des pièces par catégorie, triée par nombre décroissant.
    /// Les catégories sans aucune pièce sont omises (pas de ligne à 0/0).
    /// </summary>
    public async Task<List<CategoryStat>> GetCategoryBreakdownAsync()
    {
        var items = await _clothingService.GetItemsAsync(null, null);
        var total = items.Count;
        if (total == 0) return new List<CategoryStat>();

        return items
            .GroupBy(i => i.Category)
            .Select(g => new CategoryStat(g.Key, g.Count(), total))
            .OrderByDescending(s => s.Count)
            .ToList();
    }

    /// <summary>
    /// Palmarès des tenues portées, triées par nombre de ports décroissant.
    /// Liste vide si aucune tenue n'a jamais été marquée comme portée.
    /// </summary>
    public async Task<List<OutfitWearStat>> GetWearRankingAsync()
    {
        var history = await _wearHistoryService.GetAllAsync();
        if (history.Count == 0) return new List<OutfitWearStat>();

        return history
            .GroupBy(h => h.OutfitId)
            .Select(g => new OutfitWearStat(
                g.Key,
                g.First().OutfitNameSnapshot,
                g.Count()))
            .OrderByDescending(s => s.WearCount)
            .ToList();
    }
}
