using System.Windows.Input;
using Vesti.Models;
using Vesti.Services;
using Vesti.Infrastructure;
namespace Vesti.ViewModels.Outfits;

/// <summary>
/// Étape 3 : choix de l'occasion et enregistrement final de la tenue.
/// L'utilisateur peut passer cette étape ("Passer et enregistrer sans occasion").
/// </summary>
public class ComposeOccasionViewModel : BaseViewModel
{
    private readonly OutfitService _outfitService;

    public ComposeOccasionViewModel(OutfitService outfitService)
    {
        _outfitService = outfitService;

        Occasions = ClothingOptions.Occasions;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        SaveCommand = new Command(async () => await SaveAsync(SelectedOccasion));
        SkipSaveCommand = new Command(async () => await SaveAsync(null));
        SelectOccasionCommand = new Command<string>(o => SelectedOccasion = o);
    }

    // ── Données ──────────────────────────────────────────────────────────
    public string[] Occasions { get; }

    private string? _selectedOccasion;
    public string? SelectedOccasion
    {
        get => _selectedOccasion;
        set => SetProperty(ref _selectedOccasion, value);
    }

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand BackCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SkipSaveCommand { get; }
    public ICommand SelectOccasionCommand { get; }

    // ── Chargement ───────────────────────────────────────────────────────
    public void Load()
    {
        SelectedOccasion = ComposeState.Current.Occasion;
        ErrorMessage = null;
    }

    // ── Enregistrement ───────────────────────────────────────────────────
    private async Task SaveAsync(string? occasion)
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            ComposeState.Current.Occasion = occasion;

            var ids = ComposeState.Current.SelectedItems
                .Select(i => i.Id)
                .ToList();

            var result = await _outfitService.CreateAsync(
                ComposeState.Current.Name,
                occasion,
                ids);

            if (result.Error is not null || result.Outfit is null)
            {
                ErrorMessage = result.Error ?? "La tenue n'a pas pu être enregistrée.";
                return;
            }

            ComposeState.Current.SavedOutfit = result.Outfit;

            // Navigation vers l'écran "Tenue enregistrée"
            // On vide la pile et on navigue vers le détail
            await Shell.Current.GoToAsync(
                $"..\\..\\..\\{Routes.OutfitDetail}?id={result.Outfit.Id}");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Une erreur est survenue. Réessayez.";
            System.Diagnostics.Debug.WriteLine($"[ComposeOccasionViewModel] {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
