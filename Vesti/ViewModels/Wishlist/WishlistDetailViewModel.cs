using System.Windows.Input;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Wishlist;

/// <summary>
/// Détail d'un souhait. "Modifier cette envie" bascule la page en mode
/// édition sur place (pas de page séparée) : plus simple, et cohérent
/// avec le volume réduit de champs à modifier.
/// </summary>
[QueryProperty(nameof(ItemIdText), "id")]
public class WishlistDetailViewModel : BaseViewModel
{
    private readonly WishlistService _wishlistService;
    private readonly ClothingService _clothingService;
    private int _itemId;

    public WishlistDetailViewModel(WishlistService wishlistService, ClothingService clothingService)
    {
        _wishlistService = wishlistService;
        _clothingService = clothingService;

        Categories = ClothingOptions.Categories;
        Colors = ClothingOptions.Colors;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        StartEditCommand = new Command(() => IsEditing = true);
        CancelEditCommand = new Command(() => { IsEditing = false; ResetFieldsFromItem(); });
        SaveCommand = new Command(async () => await SaveAsync());
        MarkAcquiredCommand = new Command(async () => await MarkAcquiredAsync());
        DeleteCommand = new Command(async () => await DeleteAsync());
    }

    public string ItemIdText
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _itemId = id;
                _ = LoadAsync(id);
            }
        }
    }

    private WishlistItem? _item;
    public WishlistItem? Item
    {
        get => _item;
        private set
        {
            if (SetProperty(ref _item, value))
            {
                OnPropertyChanged(nameof(HasItem));
                OnPropertyChanged(nameof(IsAcquired));
            }
        }
    }

    public bool HasItem => Item is not null;
    public bool IsAcquired => Item?.IsAcquired == true;

    // ── Mode édition ─────────────────────────────────────────────────────
    private bool _isEditing;
    public bool IsEditing
    {
        get => _isEditing;
        private set
        {
            if (SetProperty(ref _isEditing, value))
                OnPropertyChanged(nameof(IsNotEditing));
        }
    }
    public bool IsNotEditing => !_isEditing;

    public IList<string> Categories { get; }
    public IList<string> Colors { get; }

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

    private string _reference = string.Empty;
    public string Reference { get => _reference; set => SetProperty(ref _reference, value ?? string.Empty); }

    public ICommand BackCommand { get; }
    public ICommand StartEditCommand { get; }
    public ICommand CancelEditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand MarkAcquiredCommand { get; }
    public ICommand DeleteCommand { get; }

    private async Task LoadAsync(int id)
    {
        try
        {
            Item = await _wishlistService.GetByIdAsync(id);
            if (Item is null) { ErrorMessage = "Cette envie est introuvable."; return; }
            ResetFieldsFromItem();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger cette envie.";
            System.Diagnostics.Debug.WriteLine($"[WishlistDetailViewModel] {ex}");
        }
    }

    private void ResetFieldsFromItem()
    {
        if (Item is null) return;
        Name = Item.Name;
        Category = Item.Category;
        Color = Item.Color;
        PriceText = Item.Price?.ToString() ?? string.Empty;
        Link = Item.Link ?? string.Empty;
        Reference = Item.Reference ?? string.Empty;
    }

    private async Task SaveAsync()
    {
        if (IsBusy || Item is null) return;
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

            Item.Name = Name;
            Item.Category = Category ?? string.Empty;
            Item.Color = Color;
            Item.Price = price;
            Item.Link = Link;
            Item.Reference = Reference;

            var error = await _wishlistService.UpdateAsync(Item);
            if (error is not null) { ErrorMessage = error; return; }

            IsEditing = false;
            OnPropertyChanged(nameof(Item));
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible d'enregistrer les modifications.";
            System.Diagnostics.Debug.WriteLine($"[WishlistDetailViewModel] {ex}");
        }
        finally { IsBusy = false; }
    }

    /// <summary>
    /// Transfert explicite vers le dressing : une vraie fiche ClothingItem
    /// est créée (jamais automatiquement), puis le souhait est marqué acquis.
    /// </summary>
    private async Task MarkAcquiredAsync()
    {
        if (Item is null || IsBusy) return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Marquer comme acquise",
            $"« {Item.Name} » sera ajoutée à votre dressing.",
            "Confirmer", "Annuler");

        if (!confirmed) return;

        IsBusy = true;
        try
        {
            var prepared = _wishlistService.PrepareAcquisition(Item);
            var result = await _clothingService.AddAsync(prepared);

            if (result.Error is not null)
            {
                ErrorMessage = result.Error;
                return;
            }

            await _wishlistService.MarkAcquiredAsync(Item.Id);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de marquer cette envie comme acquise.";
            System.Diagnostics.Debug.WriteLine($"[WishlistDetailViewModel] {ex}");
        }
        finally { IsBusy = false; }
    }

    private async Task DeleteAsync()
    {
        if (Item is null || IsBusy) return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Supprimer cette envie",
            $"Supprimer « {Item.Name} » de votre wishlist ?",
            "Supprimer", "Annuler");

        if (!confirmed) return;

        IsBusy = true;
        try
        {
            await _wishlistService.DeleteAsync(Item.Id);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de supprimer cette envie.";
            System.Diagnostics.Debug.WriteLine($"[WishlistDetailViewModel] {ex}");
        }
        finally { IsBusy = false; }
    }
}