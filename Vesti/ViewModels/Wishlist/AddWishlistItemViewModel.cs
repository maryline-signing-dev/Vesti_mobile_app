using System.Windows.Input;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Wishlist;

public class AddWishlistItemViewModel : BaseViewModel
{
    private readonly WishlistService _wishlistService;

    public AddWishlistItemViewModel(WishlistService wishlistService)
    {
        _wishlistService = wishlistService;

        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    public IList<string> Categories => ClothingOptions.Categories;
    public IList<string> Colors => ClothingOptions.Colors;

    private string _name = string.Empty;
    public string Name { get => _name; set => SetProperty(ref _name, value ?? string.Empty); }

    private string? _category;
    public string? Category { get => _category; set => SetProperty(ref _category, value); }

    private string? _color;
    public string? Color { get => _color; set => SetProperty(ref _color, value); }

    private string _priceText = string.Empty;
    public string PriceText { get => _priceText; set => SetProperty(ref _priceText, value ?? string.Empty); }

    private string _link = string.Empty;
    public string Link { get => _link; set => SetProperty(ref _link, value ?? string.Empty); }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private async Task SaveAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            int? price = null;
            if (!string.IsNullOrWhiteSpace(PriceText))
            {
                if (!int.TryParse(PriceText, out var p) || p < 0)
                {
                    ErrorMessage = "Le prix doit être un nombre positif.";
                    return;
                }
                price = p;
            }

            var item = new WishlistItem
            {
                Name = Name,
                Category = Category ?? string.Empty,
                Color = Color,
                Price = price,
                Link = Link
            };

            var result = await _wishlistService.AddAsync(item);
            if (result.Error is not null)
            {
                ErrorMessage = result.Error;
                return;
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Une erreur est survenue. Réessayez.";
            System.Diagnostics.Debug.WriteLine($"[AddWishlistItemViewModel] {ex}");
        }
        finally { IsBusy = false; }
    }
}