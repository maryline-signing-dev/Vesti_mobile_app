using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Services;

namespace Vesti.ViewModels.Home;

/// <summary>
/// ViewModel de l'accueil.
/// </summary>
public class HomeViewModel : BaseViewModel
{
    private readonly ProfileService _profileService;
    private readonly ClothingService _clothingService;
    private readonly WishlistService _wishlistService;
    private readonly OutfitService _outfitService;
    private readonly ScheduleService _scheduleService;
    private readonly FavoritesService _favoritesService;

    public HomeViewModel(ProfileService profileService, ClothingService clothingService, WishlistService wishlistService, OutfitService outfitService, ScheduleService scheduleService, FavoritesService favoritesService)
    {
        _profileService = profileService;
        _clothingService = clothingService;
        _wishlistService = wishlistService;
        _outfitService = outfitService;
        _scheduleService = scheduleService;
        _favoritesService = favoritesService;

        // Commande utilisée par le bouton principal (état vide ET état avec données)
        AddFirstItemCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.AddItem));

        // Alias pour cohérence avec WardrobeViewModel
        AddItemCommand = AddFirstItemCommand;

        GoToWardrobeCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.WardrobeTab));

        //  navigue vers la wishlist
        GoToWishlistCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.WishlistList));

        // Accès aux tenues depuis l'accueil
        GoToOutfitsCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.OutfitList));

        GoToFavoritesCommand = new Command(async () =>
           await Shell.Current.GoToAsync(Routes.Favorites));

        GoToAgendaCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.AgendaTab));
        
    }

    // ── Données affichées ────────────────────────────────────────────────

    private string _greeting = "Bonjour";
    public string Greeting
    {
        get => _greeting;
        private set => SetProperty(ref _greeting, value);
    }

    private int _itemCount;
    public int ItemCount
    {
        get => _itemCount;
        private set
        {
            if (SetProperty(ref _itemCount, value))
            {
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasData));
                OnPropertyChanged(nameof(ItemCountLabel));
            }
        }
    }

    private int _wishlistCount;
    public int WishlistCount
    {
        get => _wishlistCount;
        private set => SetProperty(ref _wishlistCount, value);
    }

    private int _outfitCount;
    public int OutfitCount
    {
        get => _outfitCount;
        private set
        {
            if (SetProperty(ref _outfitCount, value))
                OnPropertyChanged(nameof(OutfitCountLabel));
        }
    }

    private int _scheduledCount;
    public int ScheduledCount
    {
        get => _scheduledCount;
        private set
        {
            if (SetProperty(ref _scheduledCount, value))
                OnPropertyChanged(nameof(ScheduledCountLabel));
        }
    }
    public bool IsEmpty => _itemCount == 0;
    public bool HasData => _itemCount > 0;
    public string ItemCountLabel => _itemCount <= 1 ? "pièce" : "pièces";
    public string OutfitCountLabel => _outfitCount <= 1 ? "tenue" : "tenues";
    public string ScheduledCountLabel => _scheduledCount <= 1 ? "planifiée" : "planifiées";

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand AddFirstItemCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand GoToWardrobeCommand { get; }
    public ICommand GoToWishlistCommand { get; }
    public ICommand GoToOutfitsCommand { get; }
    public ICommand GoToFavoritesCommand { get; }
    public ICommand GoToAgendaCommand { get; }

    // ── Chargement ───────────────────────────────────────────────────────

    public async Task LoadAsync()
    {
        try
        {
            var profile = await _profileService.GetProfileAsync();
            Greeting = string.IsNullOrWhiteSpace(profile?.DisplayName)
                ? "Bonjour"
                : $"Bonjour, {profile.DisplayName}";

            ItemCount = await _clothingService.CountAsync();
            OutfitCount = await _outfitService.CountAsync();
            ScheduledCount = await _scheduleService.CountAsync();
            WishlistCount = await _wishlistService.CountActiveAsync();

            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger l'accueil.";
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] {ex}");
        }
    }
}