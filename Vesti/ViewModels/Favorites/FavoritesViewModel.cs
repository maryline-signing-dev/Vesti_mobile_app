using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Favorites;

/// <summary>
/// Regroupe les pièces et les tenues marquées favorites.
/// Chaque favori renvoie à sa fiche d'origine (ItemDetail ou OutfitDetail) —
/// aucune donnée dupliquée ici, uniquement un renvoi.
/// </summary>
public class FavoritesViewModel : BaseViewModel
{
    private readonly ClothingService _clothingService;
    private readonly OutfitService _outfitService;

    public FavoritesViewModel(ClothingService clothingService, OutfitService outfitService)
    {
        _clothingService = clothingService;
        _outfitService = outfitService;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        SelectItemCommand = new Command<ClothingItem>(async item =>
            await Shell.Current.GoToAsync($"{Routes.ItemDetail}?id={item.Id}"));
        SelectOutfitCommand = new Command<Outfit>(async outfit =>
            await Shell.Current.GoToAsync($"{Routes.OutfitDetail}?id={outfit.Id}"));
    }

    private IReadOnlyList<ClothingItem> _favoriteItems = Array.Empty<ClothingItem>();
    public IReadOnlyList<ClothingItem> FavoriteItems
    {
        get => _favoriteItems;
        private set
        {
            if (SetProperty(ref _favoriteItems, value))
                RefreshState();
        }
    }

    private IReadOnlyList<Outfit> _favoriteOutfits = Array.Empty<Outfit>();
    public IReadOnlyList<Outfit> FavoriteOutfits
    {
        get => _favoriteOutfits;
        private set
        {
            if (SetProperty(ref _favoriteOutfits, value))
                RefreshState();
        }
    }

    public bool HasItems => FavoriteItems.Count > 0;
    public bool HasOutfits => FavoriteOutfits.Count > 0;
    public bool IsEmpty => !HasItems && !HasOutfits;
    public bool HasData => HasItems || HasOutfits;

    public ICommand BackCommand { get; }
    public ICommand SelectItemCommand { get; }
    public ICommand SelectOutfitCommand { get; }

    public async Task LoadAsync()
    {
        try
        {
            FavoriteItems = await _clothingService.GetFavoritesAsync();
            FavoriteOutfits = await _outfitService.GetFavoritesAsync();
            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger vos favoris.";
            System.Diagnostics.Debug.WriteLine($"[FavoritesViewModel] {ex}");
        }
    }

    private void RefreshState()
    {
        OnPropertyChanged(nameof(HasItems));
        OnPropertyChanged(nameof(HasOutfits));
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(HasData));
    }
}