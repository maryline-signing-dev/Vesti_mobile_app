using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Agenda;

/// <summary>
/// ViewModel du formulaire "Planifier une tenue".
///
/// DEUX MODES :
///   Création : reçoit "outfitId" — on planifie cette tenue pour la 1ère fois.
///   Modification : reçoit "scheduleId" — on modifie une planification existante.
/// Le mode est déterminé par le paramètre de navigation reçu.
/// </summary>
[QueryProperty(nameof(OutfitIdText), "outfitId")]
[QueryProperty(nameof(ScheduleIdText), "scheduleId")]
public class PlanOutfitViewModel : BaseViewModel
{
    private readonly ScheduleService _scheduleService;
    private readonly OutfitService _outfitService;

    private int _outfitId;
    private int _scheduleId;
    private bool _isEditMode;

    public PlanOutfitViewModel(ScheduleService scheduleService, OutfitService outfitService)
    {
        _scheduleService = scheduleService;
        _outfitService = outfitService;

        Occasions = ClothingOptions.Occasions;

        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        SelectOccasionCommand = new Command<string>(o =>
            SelectedOccasion = SelectedOccasion == o ? null : o);
        ToggleTimeCommand = new Command(() =>
        {
            HasTime = !HasTime;
            if (!HasTime) SelectedTime = null;
            else SelectedTime ??= DateTime.Now.TimeOfDay;
        });
    }

    // ── Paramètres de navigation ─────────────────────────────────────────

    public string OutfitIdText
    {
        set
        {
            if (int.TryParse(value, out var id) && id > 0)
            {
                _outfitId = id;
                _isEditMode = false;
                _ = LoadForCreateAsync(id);
            }
        }
    }

    public string ScheduleIdText
    {
        set
        {
            if (int.TryParse(value, out var id) && id > 0)
            {
                _scheduleId = id;
                _isEditMode = true;
                _ = LoadForEditAsync(id);
            }
        }
    }

    // ── Tenue affichée en en-tête ─────────────────────────────────────────

    private Outfit? _outfit;
    public Outfit? Outfit
    {
        get => _outfit;
        private set
        {
            if (SetProperty(ref _outfit, value))
                OnPropertyChanged(nameof(HasOutfit));
        }
    }

    public bool HasOutfit => Outfit is not null;
    public bool IsEditMode => _isEditMode;

    // ── Champs du formulaire ─────────────────────────────────────────────

    private DateTime _selectedDate = DateTime.Today;
    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetProperty(ref _selectedDate, value))
                OnPropertyChanged(nameof(DateLabel));
        }
    }

    public string DateLabel => SelectedDate.ToString("d MMMM yyyy");

    private bool _hasTime;
    public bool HasTime
    {
        get => _hasTime;
        set => SetProperty(ref _hasTime, value);
    }

    private TimeSpan? _selectedTime;
    public TimeSpan? SelectedTime
    {
        get => _selectedTime;
        set => SetProperty(ref _selectedTime, value);
    }

    public string[] Occasions { get; }

    private string? _selectedOccasion;
    public string? SelectedOccasion
    {
        get => _selectedOccasion;
        set => SetProperty(ref _selectedOccasion, value);
    }

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand SelectOccasionCommand { get; }
    public ICommand ToggleTimeCommand { get; }

    // ── Chargement ───────────────────────────────────────────────────────

    private async Task LoadForCreateAsync(int outfitId)
    {
        try
        {
            Outfit = await _outfitService.GetByIdAsync(outfitId);
            if (Outfit is null) { ErrorMessage = "Cette tenue est introuvable."; return; }

            // Date présélectionnée depuis le calendrier (si on vient d'un jour cliqué)
            SelectedDate = AgendaPlanState.PreselectedDate ?? DateTime.Today;
            AgendaPlanState.PreselectedDate = null;

            SelectedOccasion = Outfit.Occasion; // reprend l'occasion de la tenue par défaut
            HasTime = false;
            SelectedTime = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger la tenue.";
            System.Diagnostics.Debug.WriteLine($"[PlanOutfitViewModel] {ex}");
        }
    }

    private async Task LoadForEditAsync(int scheduleId)
    {
        try
        {
            var schedule = await _scheduleService.GetByIdAsync(scheduleId);
            if (schedule is null) { ErrorMessage = "Cette planification est introuvable."; return; }

            Outfit = schedule.Outfit;
            SelectedDate = schedule.Date;
            SelectedTime = schedule.Time;
            HasTime = schedule.Time is not null;
            SelectedOccasion = schedule.Occasion;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger la planification.";
            System.Diagnostics.Debug.WriteLine($"[PlanOutfitViewModel] {ex}");
        }
    }

    // ── Enregistrement ───────────────────────────────────────────────────

    private async Task SaveAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var time = HasTime ? SelectedTime : null;

            if (_isEditMode)
            {
                var error = await _scheduleService.UpdateAsync(
                    _scheduleId, SelectedDate, time, SelectedOccasion);

                if (error is not null) { ErrorMessage = error; return; }

                await Shell.Current.GoToAsync(
                    $"..\\{Routes.ScheduleDetail}?id={_scheduleId}");
            }
            else
            {
                var result = await _scheduleService.CreateAsync(
                    _outfitId, SelectedDate, time, SelectedOccasion);

                if (result.Error is not null || result.Schedule is null)
                {
                    ErrorMessage = result.Error ?? "La planification n'a pas pu être enregistrée.";
                    return;
                }

                await Shell.Current.GoToAsync(
                    $"..\\{Routes.ScheduleDetail}?id={result.Schedule.Id}");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Une erreur est survenue. Réessayez.";
            System.Diagnostics.Debug.WriteLine($"[PlanOutfitViewModel] {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}