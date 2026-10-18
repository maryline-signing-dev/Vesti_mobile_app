using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Services;

namespace Vesti.ViewModels.Home;

/// <summary>
/// ViewModel de l'accueil.
/// I2 : prénom + nombre réel de pièces. Les tenues, l'agenda et la wishlist
/// s'ajouteront aux incréments suivants, sans rien afficher de fictif d'ici là.
/// </summary>
public class HomeViewModel : BaseViewModel
{
    private readonly ProfileService _profileService;
    private readonly ClothingService _clothingService;

    public HomeViewModel(ProfileService profileService, ClothingService clothingService)
    {
        _profileService = profileService;
        _clothingService = clothingService;

        AddItemCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.AddItem));

        GoToWardrobeCommand = new Command(async () =>
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
    public ICommand AddItemCommand { get; }
    public ICommand GoToWardrobeCommand { get; }

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