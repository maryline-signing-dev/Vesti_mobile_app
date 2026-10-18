using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Wardrobe;

/// <summary>
/// ViewModel du détail d'une pièce.
///
/// L'identifiant arrive par la navigation : Routes.ItemDetail + "?id=12".
/// Il est reçu sous forme de TEXTE (ItemIdText) puis converti : c'est le plus
/// fiable, car Shell transmet toujours les paramètres sous forme de chaînes.
/// </summary>
[QueryProperty(nameof(ItemIdText), "id")]
public class ItemDetailViewModel : BaseViewModel
{
    private readonly ClothingService _clothingService;
    private int _itemId;

    public ItemDetailViewModel(ClothingService clothingService)
    {
        _clothingService = clothingService;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        EditCommand = new Command(async () => await EditAsync());
        DeleteCommand = new Command(async () => await DeleteAsync());
    }

    // ── Paramètre de navigation ──────────────────────────────────────────
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

    // ── Pièce affichée ───────────────────────────────────────────────────

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
            }
        }
    }

    public bool HasItem => Item is not null;
    public bool HasColor => !string.IsNullOrWhiteSpace(Item?.Color);
    public bool HasDescription => !string.IsNullOrWhiteSpace(Item?.Description);

    public string SeasonText => OrDefault(Item?.Season, "Non renseignée");
    public string StyleText => OrDefault(Item?.Style, "Non renseigné");
    public string BrandText => OrDefault(Item?.Brand, "Non renseignée");

    private static string OrDefault(string? value, string fallback)
        => string.IsNullOrWhiteSpace(value) ? fallback : value;

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand BackCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    // ── Chargement ───────────────────────────────────────────────────────

    /// <summary>
    /// Recharge la pièce depuis la base. Appelé aussi par la page à chaque
    /// affichage, pour voir les modifications faites dans l'écran d'édition.
    /// </summary>
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

    // ── Actions ──────────────────────────────────────────────────────────

    private async Task EditAsync()
    {
        if (Item is null) return;
        await Shell.Current.GoToAsync($"{Routes.EditItem}?id={Item.Id}");
    }

    private async Task DeleteAsync()
    {
        if (Item is null || IsBusy) return;

        // Confirmation obligatoire : la suppression est définitive.
        var confirmed = await Shell.Current.DisplayAlert(
            "Supprimer la pièce",
            $"Supprimer « {Item.Name} » ? Cette action est définitive.",
            "Supprimer",
            "Annuler");

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
        finally
        {
            IsBusy = false;
        }
    }
}