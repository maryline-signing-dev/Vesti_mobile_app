using System.Windows.Input;
using Vesti.Services;

namespace Vesti.ViewModels.Home;

/// <summary>
/// ViewModel de l'accueil.
/// En I1 : affiche le nom de l'utilisateur et l'état vide.
/// En I2+ : les compteurs et les sections seront ajoutés ici.
/// </summary>
public class HomeViewModel : BaseViewModel
{
    private readonly ProfileService _profileService;

    public HomeViewModel(ProfileService profileService)
    {
        _profileService = profileService;

        AddFirstItemCommand = new Command(async () =>
            await Shell.Current.GoToAsync("wardrobe/add"));
    }

    // ── Données affichées ────────────────────────────────────────────────
    private string _displayName = string.Empty;
    public string DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }

    /// <summary>
    /// Vrai tant que l'utilisateur n'a aucune pièce.
    /// Sera mis à jour en I2 via ClothingService.
    /// </summary>
    public bool IsEmpty => true; // provisoire — remplacé en I2

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand AddFirstItemCommand { get; }

    // ── Cycle de vie ─────────────────────────────────────────────────────
    /// <summary>
    /// Appelé depuis HomePage.OnAppearing() pour rafraîchir les données.
    /// </summary>
    public async Task LoadAsync()
    {
        var profile = await _profileService.GetProfileAsync();
        DisplayName = profile?.DisplayName ?? string.Empty;
    }
}
