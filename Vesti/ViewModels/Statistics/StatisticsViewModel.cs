using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Services;

namespace Vesti.ViewModels.Statistics;

public class StatisticsViewModel : BaseViewModel
{
    private readonly StatsService _statsService;

    public StatisticsViewModel(StatsService statsService)
    {
        _statsService = statsService;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        GoToWardrobeCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.WardrobeTab));
    }

    private int _pieceCount;
    public int PieceCount
    {
        get => _pieceCount;
        private set
        {
            if (SetProperty(ref _pieceCount, value))
            {
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasData));
            }
        }
    }

    private int _outfitCount;
    public int OutfitCount { get => _outfitCount; private set => SetProperty(ref _outfitCount, value); }

    private int _scheduledCount;
    public int ScheduledCount { get => _scheduledCount; private set => SetProperty(ref _scheduledCount, value); }

    public bool IsEmpty => _pieceCount == 0;
    public bool HasData => _pieceCount > 0;

    private IReadOnlyList<CategoryStat> _categories = Array.Empty<CategoryStat>();
    public IReadOnlyList<CategoryStat> Categories
    {
        get => _categories;
        private set => SetProperty(ref _categories, value);
    }

    private IReadOnlyList<OutfitWearStat> _wearRanking = Array.Empty<OutfitWearStat>();
    public IReadOnlyList<OutfitWearStat> WearRanking
    {
        get => _wearRanking;
        private set
        {
            if (SetProperty(ref _wearRanking, value))
                OnPropertyChanged(nameof(HasWearData));
        }
    }

    public bool HasWearData => WearRanking.Count > 0;
    public OutfitWearStat? MostWorn => WearRanking.Count > 0 ? WearRanking[0] : null;
    public OutfitWearStat? LeastWorn => WearRanking.Count > 0 ? WearRanking[^1] : null;

    /// <summary>Vrai seulement si plus/moins portée sont deux tenues différentes.</summary>
    public bool HasDistinctLeastWorn => WearRanking.Count > 1;

    public ICommand BackCommand { get; }
    public ICommand GoToWardrobeCommand { get; }

    public async Task LoadAsync()
    {
        try
        {
            var overview = await _statsService.GetOverviewAsync();
            PieceCount = overview.Pieces;
            OutfitCount = overview.Outfits;
            ScheduledCount = overview.Scheduled;

            Categories = await _statsService.GetCategoryBreakdownAsync();
            WearRanking = await _statsService.GetWearRankingAsync();

            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger les statistiques.";
            System.Diagnostics.Debug.WriteLine($"[StatisticsViewModel] {ex}");
        }
    }
}
