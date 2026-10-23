using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Wardrobe;

/// <summary>
/// ViewModel de "Ajouter une pièce".
/// La page a deux états : le formulaire (IsNotSaved) puis la confirmation (IsSaved).
/// - Reçoit IImageStore par injection.
/// - Expose PickPhotoCommand (galerie), CapturePhotoCommand (caméra).
/// - Expose ImagePath (chemin relatif) et HasPhoto pour la vue.
/// - SaveAsync transmet l'ImagePath à la pièce avant de l'enregistrer.
/// - Reset() remet aussi l'image à null.
/// Tout le reste est inchangé.
/// </summary>
public class AddItemViewModel : BaseViewModel
{
    private readonly ClothingService _clothingService;
    private readonly IImageStore _imageStore;

    public AddItemViewModel(ClothingService clothingService, IImageStore imageStore)
    {
        _clothingService = clothingService;
        _imageStore = imageStore;

        SaveCommand = new Command(async () => await SaveAsync());
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        ViewWardrobeCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.TabWardrobe));
        AddAnotherCommand = new Command(Reset);
        PickPhotoCommand = new Command(async () => await PickPhotoAsync());
        CapturePhotoCommand = new Command(async () => await CapturePhotoAsync());
    }

    // ── Listes ───────────────────────────────────────────────────────────
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

    /// <summary>Couleur choisie dans la liste prédéfinie (Picker).</summary>
    private string? _selectedColor;
    public string? SelectedColor
    {
        get => _selectedColor;
        set
        {
            if (SetProperty(ref _selectedColor, value))
                OnPropertyChanged(nameof(EffectiveColor));
        }
    }

    /// <summary>Couleur libre tapée par l'utilisateur — prime sur SelectedColor.</summary>
    private string _customColor = string.Empty;
    public string CustomColor
    {
        get => _customColor;
        set
        {
            if (SetProperty(ref _customColor, value ?? string.Empty))
                OnPropertyChanged(nameof(EffectiveColor));
        }
    }

    /// <summary>
    /// Couleur réellement enregistrée : la saisie libre prime si non vide,
    /// sinon la sélection du Picker.
    /// </summary>
    public string? EffectiveColor
        => string.IsNullOrWhiteSpace(_customColor) ? _selectedColor : _customColor.Trim();


    // ── Photo ─────────────────────────────────────────────────────────────

    /// <summary>Chemin RELATIF de l'image choisie (null si aucune).</summary>
    private string? _imagePath;
    public string? ImagePath
    {
        get => _imagePath;
        private set
        {
            if (SetProperty(ref _imagePath, value))
                OnPropertyChanged(nameof(HasPhoto));
        }
    }

    /// <summary>Chemin ABSOLU pour affichage dans la vue (ImageSource).</summary>
    public string? FullImagePath => _imageStore.GetFullPath(_imagePath);

    /// <summary>Vrai si une photo a été choisie.</summary>
    public bool HasPhoto => _imagePath is not null;

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

    public string? SavedItemFullImagePath
         => _imageStore.GetFullPath(_savedItem?.ImagePath);

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
    public ICommand PickPhotoCommand { get; }
    public ICommand CapturePhotoCommand { get; }

    // ── Gestion de la photo ───────────────────────────────────────────────

    private async Task PickPhotoAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var path = await _imageStore.PickFromGalleryAsync();
            if (path is not null)
            {
                ImagePath = path;
                OnPropertyChanged(nameof(FullImagePath));
            }
        }
        finally { IsBusy = false; }
    }

    private async Task CapturePhotoAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var path = await _imageStore.CaptureFromCameraAsync();
            if (path is not null)
            {
                ImagePath = path;
                OnPropertyChanged(nameof(FullImagePath));
            }
        }
        finally { IsBusy = false; }
    }

    // ── Sauvegarde ────────────────────────────────────────────────────────

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
                Color = EffectiveColor,
                ImagePath = _imagePath      // I3 : photo transmise
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

    private void Reset()
    {
        Name = string.Empty;
        Category = null;
        SelectedColor = null;
        CustomColor = string.Empty;
        ImagePath = null;
        ErrorMessage = null;
        SavedItem = null;
        ConfirmationText = string.Empty;
        IsSaved = false;
        OnPropertyChanged(nameof(FullImagePath));
    }
}
