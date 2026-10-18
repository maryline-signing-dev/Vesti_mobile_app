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

    public HomeViewModel(ProfileService profileService, ClothingService clothingService)
    {
        _profileService = profileService;
        _clothingService = clothingService;

        // Commande utilisée par le bouton principal (état vide ET état avec données)
        AddFirstItemCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.AddItem));

        // Alias pour cohérence avec WardrobeViewModel
        AddItemCommand = AddFirstItemCommand;

        GoToWardrobeCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.TabWardrobe));

        // En attendant la wishlist (I6), navigue vers le dressing
        GoToWishlistCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.TabWardrobe));
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

    public bool IsEmpty => _itemCount == 0;
    public bool HasData => _itemCount > 0;
    public string ItemCountLabel => _itemCount <= 1 ? "pièce" : "pièces";

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand AddFirstItemCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand GoToWardrobeCommand { get; }
    public ICommand GoToWishlistCommand { get; }

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
            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger l'accueil.";
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] {ex}");
        }
    }
}