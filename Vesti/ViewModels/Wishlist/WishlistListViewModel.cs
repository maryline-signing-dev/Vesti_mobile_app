using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Wishlist;

public class WishlistListViewModel : BaseViewModel
{
    private readonly WishlistService _wishlistService;
    private int _loadVersion;

    public WishlistListViewModel(WishlistService wishlistService)
    {
        _wishlistService = wishlistService;

        AddCommand = new Command(async () => await Shell.Current.GoToAsync(Routes.AddWishlist));
        ClearSearchCommand = new Command(() => SearchText = string.Empty);
        SelectItemCommand = new Command<WishlistItem>(async item =>
            await Shell.Current.GoToAsync($"{Routes.WishlistDetail}?id={item.Id}"));
    }

    private IReadOnlyList<WishlistItem> _items = Array.Empty<WishlistItem>();
    public IReadOnlyList<WishlistItem> Items
    {
        get => _items;
        private set
        {
            if (SetProperty(ref _items, value))
            {
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasItems));
                OnPropertyChanged(nameof(ResultLabel));
            }
        }
    }

    public bool IsEmpty => Items.Count == 0 && string.IsNullOrEmpty(SearchText);
    public bool HasItems => Items.Count > 0;
    public bool IsSearchActive => !string.IsNullOrEmpty(SearchText);
    public string ResultLabel => Items.Count == 1 ? "1 résultat" : $"{Items.Count} résultats";

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value ?? string.Empty))
            {
                OnPropertyChanged(nameof(IsSearchActive));
                _ = LoadAsync();
            }
        }
    }

    public ICommand AddCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ICommand SelectItemCommand { get; }

    public async Task LoadAsync()
    {
        var version = ++_loadVersion;
        try
        {
            var items = await _wishlistService.GetActiveAsync(SearchText);
            if (version != _loadVersion) return;

            Items = items;
            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            if (version == _loadVersion)
                ErrorMessage = "Impossible de charger votre wishlist.";
            System.Diagnostics.Debug.WriteLine($"[WishlistListViewModel] {ex}");
        }
    }
}