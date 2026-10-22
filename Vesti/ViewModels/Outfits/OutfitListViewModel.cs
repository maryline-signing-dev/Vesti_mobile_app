using System.Windows.Input;
using Vesti.Models;
using Vesti.Services;
using Vesti.Infrastructure;

namespace Vesti.ViewModels.Outfits;

public class OutfitListViewModel : BaseViewModel
{
    private readonly OutfitService _outfitService;

    public OutfitListViewModel(OutfitService outfitService)
    {
        _outfitService = outfitService;

        ComposeCommand = new Command(async () => await StartComposeAsync());
        SelectOutfitCommand = new Command<Outfit>(async o => await OpenOutfitAsync(o));
    }

    private IReadOnlyList<Outfit> _outfits = Array.Empty<Outfit>();
    public IReadOnlyList<Outfit> Outfits
    {
        get => _outfits;
        private set
        {
            if (SetProperty(ref _outfits, value))
            {
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasOutfits));
            }
        }
    }

    public bool IsEmpty => Outfits.Count == 0;
    public bool HasOutfits => Outfits.Count > 0;

    public ICommand ComposeCommand { get; }
    public ICommand SelectOutfitCommand { get; }

    public async Task LoadAsync()
    {
        try
        {
            Outfits = await _outfitService.GetAllAsync();
            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger vos tenues.";
            System.Diagnostics.Debug.WriteLine($"[OutfitListViewModel] {ex}");
        }
    }

    private async Task StartComposeAsync()
    {
        ComposeState.Current.Reset();
        await Shell.Current.GoToAsync(Routes.ComposeSelection);
    }

    private async Task OpenOutfitAsync(Outfit outfit)
        => await Shell.Current.GoToAsync($"{Routes.OutfitDetail}?id={outfit.Id}");
}
