using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Wardrobe;

/// <summary>
/// ViewModel de "Ajouter une pièce".
/// La page a deux états : le formulaire (IsNotSaved) puis la confirmation (IsSaved).
/// I3 ajoutera ici le choix de la photo.
/// </summary>
public class AddItemViewModel : BaseViewModel
{
    private readonly ClothingService _clothingService;

    public AddItemViewModel(ClothingService clothingService)
    {
        _clothingService = clothingService;

        SaveCommand = new Command(async () => await SaveAsync());
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        ViewWardrobeCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.TabWardrobe));
        AddAnotherCommand = new Command(Reset);
    }

    // ── Listes des sélecteurs ────────────────────────────────────────────
    public IList<string> Categories => ClothingOptions.Categories;
    public IList<string> Colors => ClothingOptions.Colors;

    // ── Champs du formulaire ─────────────────────────────────────────────

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value ?? string.Empty);
    }

    private string? _category;
    public string? Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    private string? _color;
    public string? Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

    // ── État de la page ──────────────────────────────────────────────────

    private bool _isSaved;
    public bool IsSaved
    {
        get => _isSaved;
        private set
        {
            if (SetProperty(ref _isSaved, value))
                OnPropertyChanged(nameof(IsNotSaved));
        }
    }

    public bool IsNotSaved => !_isSaved;

    private ClothingItem? _savedItem;
    public ClothingItem? SavedItem
    {
        get => _savedItem;
        private set => SetProperty(ref _savedItem, value);
    }

    private string _confirmationText = string.Empty;
    public string ConfirmationText
    {
        get => _confirmationText;
        private set => SetProperty(ref _confirmationText, value);
    }

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand SaveCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand ViewWardrobeCommand { get; }
    public ICommand AddAnotherCommand { get; }

    // ── Logique ──────────────────────────────────────────────────────────

    private async Task SaveAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var item = new ClothingItem
            {
                Name = Name,
                Category = Category ?? string.Empty,
                Color = Color
            };

            var result = await _clothingService.AddAsync(item);

            if (result.Error is not null || result.Item is null)
            {
                ErrorMessage = result.Error ?? "La pièce n'a pas pu être enregistrée.";
                return;
            }

            var count = await _clothingService.CountAsync();

            SavedItem = result.Item;
            ConfirmationText = count == 1
                ? $"« {result.Item.Name} » est maintenant dans votre dressing. C'est votre première pièce."
                : $"« {result.Item.Name} » est maintenant dans votre dressing, qui contient {count} pièces.";
            IsSaved = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Une erreur est survenue. Réessayez.";
            System.Diagnostics.Debug.WriteLine($"[AddItemViewModel] {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>Retour au formulaire vide ("Ajouter une autre pièce").</summary>
    private void Reset()
    {
        Name = string.Empty;
        Category = null;
        Color = null;
        ErrorMessage = null;
        SavedItem = null;
        ConfirmationText = string.Empty;
        IsSaved = false;
    }
}