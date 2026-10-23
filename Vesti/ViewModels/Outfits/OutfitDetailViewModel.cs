using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Outfits;

[QueryProperty(nameof(OutfitIdText), "id")]
public class OutfitDetailViewModel : BaseViewModel
{
    private readonly OutfitService _outfitService;
    private int _outfitId;

    public OutfitDetailViewModel(OutfitService outfitService)
    {
        _outfitService = outfitService;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        DeleteCommand = new Command(async () => await DeleteAsync());
        AddToAgendaCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.TabAgenda));

        GoToOutfitListCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync(Routes.TabWardrobe);
            await Shell.Current.GoToAsync(Routes.OutfitList);
        });
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
            }
        }
    }

    public bool HasOutfit => Outfit is not null;
    public bool HasOccasion => !string.IsNullOrWhiteSpace(Outfit?.Occasion);
    public string OccasionText => Outfit?.Occasion ?? "Sans occasion";

    public ICommand BackCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand AddToAgendaCommand { get; }
    public ICommand GoToOutfitListCommand { get; }

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

    private async Task DeleteAsync()
    {
        if (Outfit is null || IsBusy) return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Supprimer la tenue",
            $"Supprimer « {Outfit.DisplayName} » ? Les pièces ne seront pas supprimées.",
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
