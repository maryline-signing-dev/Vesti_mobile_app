using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Outfits;

[QueryProperty(nameof(OutfitIdText), "id")]
public class OutfitDetailViewModel : BaseViewModel
{
    private readonly OutfitService _outfitService;
    private readonly WearHistoryService _wearHistoryService;
    private int _outfitId;

    public OutfitDetailViewModel(OutfitService outfitService, WearHistoryService warHistoryService)
    {
        _outfitService = outfitService;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        DeleteCommand = new Command(async () => await DeleteAsync());

        // navigue vers le formulaire de planification réel
        AddToAgendaCommand = new Command(async () =>
            await Shell.Current.GoToAsync($"{Routes.PlanOutfit}?outfitId={_outfitId}"));
        GoToOutfitListCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync(Routes.WardrobeTab);
            await Shell.Current.GoToAsync(Routes.OutfitList);
        });

        ToggleFavoriteCommand = new Command(async () => await ToggleFavoriteAsync());
        MarkWornCommand = new Command(async () => await MarkWornAsync());
    }
    public string OutfitIdText
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _outfitId = id;
                _ = LoadAsync();
            }
        }
    }

    private Outfit? _outfit;
    public Outfit? Outfit
    {
        get => _outfit;
        private set
        {
            if (SetProperty(ref _outfit, value))
            {
                OnPropertyChanged(nameof(HasOutfit));
                OnPropertyChanged(nameof(HasOccasion));
                OnPropertyChanged(nameof(OccasionText));
                OnPropertyChanged(nameof(IsFavorite));
            }
        }
    }

    public bool HasOutfit => Outfit is not null;
    public bool HasOccasion => !string.IsNullOrWhiteSpace(Outfit?.Occasion);
    public string OccasionText => Outfit?.Occasion ?? "Sans occasion";
    public bool IsFavorite => Outfit?.IsFavorite == true;
    private string? _wornConfirmation;
    public string? WornConfirmation
    {
        get => _wornConfirmation;
        private set
        {
            if (SetProperty(ref _wornConfirmation, value))
                OnPropertyChanged(nameof(HasWornConfirmation));
        }
    }
    public bool HasWornConfirmation => !string.IsNullOrEmpty(_wornConfirmation);
    public ICommand BackCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand AddToAgendaCommand { get; }
    public ICommand GoToOutfitListCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand MarkWornCommand { get; }


    private async Task MarkWornAsync()
    {
        if (Outfit is null || IsBusy) return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Marquer comme portée",
            $"Enregistrer que vous portez « {Outfit.DisplayName} » aujourd'hui ?",
            "Confirmer", "Annuler");

        if (!confirmed) return;

        IsBusy = true;
        try
        {
            var error = await _wearHistoryService.MarkWornAsync(Outfit.Id);
            if (error is not null) { ErrorMessage = error; return; }

            WornConfirmation = $"Portée aujourd'hui, {DateTime.Now:d MMMM}.";
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible d'enregistrer cette information.";
            System.Diagnostics.Debug.WriteLine($"[OutfitDetailViewModel] {ex}");
        }
        finally { IsBusy = false; }
    }

    public async Task LoadAsync()
    {
        if (_outfitId <= 0) return;
        try
        {
            Outfit = await _outfitService.GetByIdAsync(_outfitId);
            ErrorMessage = Outfit is null ? "Cette tenue est introuvable." : null;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger la tenue.";
            System.Diagnostics.Debug.WriteLine($"[OutfitDetailViewModel] {ex}");
        }
    }

    private async Task ToggleFavoriteAsync()
    {
        if (Outfit is null) return;
        var newState = await _outfitService.ToggleFavoriteAsync(Outfit.Id);
        Outfit.IsFavorite = newState;
        OnPropertyChanged(nameof(IsFavorite));
    }

    private async Task DeleteAsync()
    {
        if (Outfit is null || IsBusy) return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Supprimer la tenue",
            $"Supprimer « {Outfit.DisplayName} » ? Les pièces ne seront pas supprimées. Les planifications de cette tenue seront aussi retirées de l'agenda.",
            "Supprimer", "Annuler");

        if (!confirmed) return;

        IsBusy = true;
        try
        {
            await _outfitService.DeleteOutfitAsync(Outfit.Id);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de supprimer la tenue.";
            System.Diagnostics.Debug.WriteLine($"[OutfitDetailViewModel] {ex}");
        }
        finally { IsBusy = false; }
    }
}