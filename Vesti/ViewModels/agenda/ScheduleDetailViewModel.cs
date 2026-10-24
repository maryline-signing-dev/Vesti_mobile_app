using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Agenda;

[QueryProperty(nameof(ScheduleIdText), "id")]
public class ScheduleDetailViewModel : BaseViewModel
{
    private readonly ScheduleService _scheduleService;
    private int _scheduleId;

    public ScheduleDetailViewModel(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        EditCommand = new Command(async () =>
            await Shell.Current.GoToAsync($"{Routes.PlanOutfit}?scheduleId={_scheduleId}"));
        RemoveCommand = new Command(async () => await RemoveAsync());
    }

    public string ScheduleIdText
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _scheduleId = id;
                _ = LoadAsync();
            }
        }
    }

    private OutfitSchedule? _schedule;
    public OutfitSchedule? Schedule
    {
        get => _schedule;
        private set
        {
            if (SetProperty(ref _schedule, value))
            {
                OnPropertyChanged(nameof(HasSchedule));
                OnPropertyChanged(nameof(HasOccasion));
                OnPropertyChanged(nameof(DateLabel));
            }
        }
    }

    public bool HasSchedule => Schedule is not null;
    public bool HasOccasion => !string.IsNullOrWhiteSpace(Schedule?.Occasion);
    public string DateLabel => Schedule?.Date.ToString("d MMMM yyyy") ?? string.Empty;

    public ICommand BackCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand RemoveCommand { get; }

    public async Task LoadAsync()
    {
        if (_scheduleId <= 0) return;
        try
        {
            Schedule = await _scheduleService.GetByIdAsync(_scheduleId);
            ErrorMessage = Schedule is null ? "Cette planification est introuvable." : null;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger la planification.";
            System.Diagnostics.Debug.WriteLine($"[ScheduleDetailViewModel] {ex}");
        }
    }

    private async Task RemoveAsync()
    {
        if (Schedule is null || IsBusy) return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Retirer de l'agenda",
            "Cette tenue restera enregistrée dans Mes tenues.",
            "Retirer", "Annuler");

        if (!confirmed) return;

        IsBusy = true;
        try
        {
            await _scheduleService.DeleteAsync(Schedule.Id);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de retirer cette planification.";
            System.Diagnostics.Debug.WriteLine($"[ScheduleDetailViewModel] {ex}");
        }
        finally { IsBusy = false; }
    }
}