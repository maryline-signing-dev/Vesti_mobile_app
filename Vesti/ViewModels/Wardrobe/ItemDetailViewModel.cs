using System.Windows.Input;
using Vesti.Models;
using Vesti.Services;
using Vesti.Infrastructure;

namespace Vesti.ViewModels.Wardrobe;

/// <summary>
/// ViewModel du détail d'une pièce.
/// L'identifiant arrive par la navigation : Routes.ItemDetail + "?id=12".
/// - Il est reçu sous forme de TEXTE (ItemIdText) puis converti : c'est le plus
/// fiable, car Shell transmet toujours les paramètres sous forme de chaînes.
/// - Reçoit IImageStore par injection.
/// - Expose HasPhoto et FullImagePath pour l'affichage conditionnel de la photo.
/// </summary>
/// 
[QueryProperty(nameof(ItemIdText), "id")]
public class ItemDetailViewModel : BaseViewModel
{
    private readonly ClothingService _clothingService;
    private readonly IImageStore _imageStore;
    private int _itemId;

    public ItemDetailViewModel(ClothingService clothingService, IImageStore imageStore)
    {
        _clothingService = clothingService;
        _imageStore = imageStore;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        EditCommand = new Command(async () => await EditAsync());
        DeleteCommand = new Command(async () => await DeleteAsync());
    }

    public string ItemIdText
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _itemId = id;
                _ = LoadAsync();
            }
        }
    }

    private ClothingItem? _item;
    public ClothingItem? Item
    {
        get => _item;
        private set
        {
            if (SetProperty(ref _item, value))
            {
                OnPropertyChanged(nameof(HasItem));
                OnPropertyChanged(nameof(HasColor));
                OnPropertyChanged(nameof(HasDescription));
                OnPropertyChanged(nameof(SeasonText));
                OnPropertyChanged(nameof(StyleText));
                OnPropertyChanged(nameof(BrandText));
                OnPropertyChanged(nameof(HasPhoto));
                OnPropertyChanged(nameof(FullImagePath));
            }
        }
    }

    public bool HasItem => Item is not null;
    public bool HasColor => !string.IsNullOrWhiteSpace(Item?.Color);
    public bool HasDescription => !string.IsNullOrWhiteSpace(Item?.Description);

    //  photo
    public bool HasPhoto => _imageStore.GetFullPath(Item?.ImagePath) is not null;
    public string? FullImagePath => _imageStore.GetFullPath(Item?.ImagePath);

    public string SeasonText => OrDefault(Item?.Season, "Non renseignée");
    public string StyleText => OrDefault(Item?.Style, "Non renseigné");
    public string BrandText => OrDefault(Item?.Brand, "Non renseignée");

    private static string OrDefault(string? value, string fallback)
        => string.IsNullOrWhiteSpace(value) ? fallback : value;

    public ICommand BackCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public async Task LoadAsync()
    {
        if (_itemId <= 0) return;
        try
        {
            var item = await _clothingService.GetByIdAsync(_itemId);
            Item = item;
            ErrorMessage = item is null ? "Cette pièce est introuvable." : null;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger la pièce.";
            System.Diagnostics.Debug.WriteLine($"[ItemDetailViewModel] {ex}");
        }
    }

    private async Task EditAsync()
    {
        if (Item is null) return;
        await Shell.Current.GoToAsync($"{Routes.EditItem}?id={Item.Id}");
    }

    private async Task DeleteAsync()
    {
        if (Item is null || IsBusy) return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Supprimer la pièce",
            $"Supprimer « {Item.Name} » ? Cette action est définitive.",
            "Supprimer", "Annuler");

        if (!confirmed) return;

        IsBusy = true;
        try
        {
            await _clothingService.DeleteAsync(Item.Id);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de supprimer la pièce.";
            System.Diagnostics.Debug.WriteLine($"[ItemDetailViewModel] {ex}");
        }
        finally { IsBusy = false; }
    }
}
