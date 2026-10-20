using System.Windows.Input;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Wardrobe;

/// <summary>
/// ViewModel de "Modifier la pièce".
/// - Charge la pièce, expose ses champs modifiables, enregistre via le service.
/// - Reçoit IImageStore par injection.
/// - Expose PickPhotoCommand, CapturePhotoCommand.
/// - Charge et expose le chemin de la photo existante.
/// - SaveAsync transmet l'ImagePath mis à jour à ClothingService.UpdateAsync,
///   qui supprime l'ancien fichier si nécessaire.
/// </summary>
[QueryProperty(nameof(ItemIdText), "id")]
public class EditItemViewModel : BaseViewModel
{
    private readonly ClothingService _clothingService;
    private readonly IImageStore _imageStore;
    private ClothingItem? _original;

    public EditItemViewModel(ClothingService clothingService, IImageStore imageStore)
    {
        _clothingService = clothingService;
        _imageStore = imageStore;

        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        PickPhotoCommand = new Command(async () => await PickPhotoAsync());
        CapturePhotoCommand = new Command(async () => await CapturePhotoAsync());
    }

    // ── Paramètre de navigation ──────────────────────────────────────────
    public string ItemIdText
    {
        set
        {
            if (int.TryParse(value, out var id))
                _ = LoadAsync(id);
        }
    }

    // ── Listes ───────────────────────────────────────────────────────────
    public IList<string> Categories => ClothingOptions.Categories;
    public IList<string> Colors => ClothingOptions.Colors;

    // ── Champs modifiables ───────────────────────────────────────────────

    private string _name = string.Empty;
    public string Name { get => _name; set => SetProperty(ref _name, value ?? string.Empty); }

    private string? _category;
    public string? Category { get => _category; set => SetProperty(ref _category, value); }

    private string? _color;
    public string? Color { get => _color; set => SetProperty(ref _color, value); }

    private string _season = string.Empty;
    public string Season { get => _season; set => SetProperty(ref _season, value ?? string.Empty); }

    private string _style = string.Empty;
    public string Style { get => _style; set => SetProperty(ref _style, value ?? string.Empty); }

    private string _brand = string.Empty;
    public string Brand { get => _brand; set => SetProperty(ref _brand, value ?? string.Empty); }

    // ── Photo ─────────────────────────────────────────────────────────────

    private string? _imagePath;
    public string? ImagePath
    {
        get => _imagePath;
        private set
        {
            if (SetProperty(ref _imagePath, value))
            {
                OnPropertyChanged(nameof(HasPhoto));
                OnPropertyChanged(nameof(FullImagePath));
            }
        }
    }

    public string? FullImagePath => _imageStore.GetFullPath(_imagePath);
    public bool HasPhoto => _imagePath is not null;

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand PickPhotoCommand { get; }
    public ICommand CapturePhotoCommand { get; }

    // ── Chargement ───────────────────────────────────────────────────────

    private async Task LoadAsync(int id)
    {
        try
        {
            _original = await _clothingService.GetByIdAsync(id);
            if (_original is null) { ErrorMessage = "Cette pièce est introuvable."; return; }

            Name = _original.Name;
            Category = _original.Category;
            Color = _original.Color;
            Season = _original.Season ?? string.Empty;
            Style = _original.Style ?? string.Empty;
            Brand = _original.Brand ?? string.Empty;
            ImagePath = _original.ImagePath;    // charger la photo existante
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger la pièce.";
            System.Diagnostics.Debug.WriteLine($"[EditItemViewModel] {ex}");
        }
    }

    // ── Gestion de la photo ───────────────────────────────────────────────

    private async Task PickPhotoAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var path = await _imageStore.PickFromGalleryAsync();
            if (path is not null) ImagePath = path;
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
            if (path is not null) ImagePath = path;
        }
        finally { IsBusy = false; }
    }

    // ── Sauvegarde ────────────────────────────────────────────────────────

    private async Task SaveAsync()
    {
        if (IsBusy || _original is null) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            _original.Name = Name;
            _original.Category = Category ?? string.Empty;
            _original.Color = Color;
            _original.Season = Season;
            _original.Style = Style;
            _original.Brand = Brand;
            _original.ImagePath = _imagePath;   // I3 : photo mise à jour

            var error = await _clothingService.UpdateAsync(_original);
            if (error is not null) { ErrorMessage = error; return; }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible d'enregistrer les modifications.";
            System.Diagnostics.Debug.WriteLine($"[EditItemViewModel] {ex}");
        }
        finally { IsBusy = false; }
    }
}
