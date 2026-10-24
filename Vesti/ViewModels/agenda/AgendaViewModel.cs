using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Agenda;

/// <summary>
/// ViewModel de l'onglet Agenda.
///
/// ÉTATS :
///   IsEmpty  : aucune planification n'existe nulle part → état d'invitation
///   HasData  : au moins une planification existe → calendrier + liste du jour
///
/// Le calendrier affiche une grille de 42 cases (6 semaines) pour le mois
/// courant, en commençant le lundi (convention française). Les jours hors
/// du mois affiché restent visibles mais grisés, comme dans la maquette.
/// </summary>
public class AgendaViewModel : BaseViewModel
{
    private readonly ScheduleService _scheduleService;
    private static readonly CultureInfo FrenchCulture = new("fr-FR");

    public AgendaViewModel(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;

        Days = new ObservableCollection<CalendarDay>();
        SchedulesForSelectedDate = new ObservableCollection<OutfitSchedule>();

        PrevMonthCommand = new Command(async () => await ChangeMonthAsync(-1));
        NextMonthCommand = new Command(async () => await ChangeMonthAsync(1));
        SelectDayCommand = new Command<CalendarDay>(async d => await SelectDayAsync(d));
        PlanCommand = new Command(async () =>
        {
            AgendaPlanState.IsSelectingForSchedule = true;
            await Shell.Current.GoToAsync(Routes.OutfitList);
        });
        SelectScheduleCommand = new Command<OutfitSchedule>(async s =>
            await Shell.Current.GoToAsync($"{Routes.ScheduleDetail}?id={s.Id}"));
    }

    // ── État global ──────────────────────────────────────────────────────
    private int _totalScheduleCount;
    public bool IsEmpty => _totalScheduleCount == 0;
    public bool HasData => _totalScheduleCount > 0;

    // ── Mois affiché ─────────────────────────────────────────────────────
    private DateTime _currentMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    public string MonthLabel
        => _currentMonth.ToString("MMMM yyyy", FrenchCulture);

    public ObservableCollection<CalendarDay> Days { get; }

    // ── Jour sélectionné ─────────────────────────────────────────────────
    private DateTime _selectedDate = DateTime.Today;
    public DateTime SelectedDate
    {
        get => _selectedDate;
        private set
        {
            if (SetProperty(ref _selectedDate, value))
                OnPropertyChanged(nameof(SelectedDateLabel));
        }
    }

    public string SelectedDateLabel
        => _selectedDate.ToString("dddd d MMMM", FrenchCulture) is { Length: > 0 } s
            ? char.ToUpper(s[0], FrenchCulture) + s[1..]
            : string.Empty;

    public ObservableCollection<OutfitSchedule> SchedulesForSelectedDate { get; }

    public bool HasSchedulesForSelectedDate => SchedulesForSelectedDate.Count > 0;
    public string SchedulesCountLabel => SchedulesForSelectedDate.Count == 1
        ? "1 tenue"
        : $"{SchedulesForSelectedDate.Count} tenues";

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand PrevMonthCommand { get; }
    public ICommand NextMonthCommand { get; }
    public ICommand SelectDayCommand { get; }
    public ICommand PlanCommand { get; }
    public ICommand SelectScheduleCommand { get; }

    // ── Chargement ───────────────────────────────────────────────────────

    public async Task LoadAsync()
    {
        try
        {
            _totalScheduleCount = await _scheduleService.CountAsync();
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(HasData));

            if (_totalScheduleCount > 0)
            {
                await BuildCalendarAsync();
                await LoadSelectedDateAsync();
            }

            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger l'agenda.";
            System.Diagnostics.Debug.WriteLine($"[AgendaViewModel] {ex}");
        }
    }

    private async Task ChangeMonthAsync(int delta)
    {
        _currentMonth = _currentMonth.AddMonths(delta);
        OnPropertyChanged(nameof(MonthLabel));
        await BuildCalendarAsync();
    }

    private async Task SelectDayAsync(CalendarDay day)
    {
        SelectedDate = day.Date;
        foreach (var d in Days) d.IsSelected = d.Date == day.Date;
        await LoadSelectedDateAsync();
    }

    /// <summary>Construit la grille de 42 jours et marque ceux qui ont une planification.</summary>
    private async Task BuildCalendarAsync()
    {
        var scheduledDates = await _scheduleService
            .GetScheduledDatesForMonthAsync(_currentMonth.Year, _currentMonth.Month);

        // Premier jour affiché = lundi de la semaine contenant le 1er du mois
        var firstOfMonth = _currentMonth;
        int offset = ((int)firstOfMonth.DayOfWeek + 6) % 7; // lundi = 0
        var firstCell = firstOfMonth.AddDays(-offset);

        Days.Clear();
        for (int i = 0; i < 42; i++)
        {
            var date = firstCell.AddDays(i);
            Days.Add(new CalendarDay
            {
                Date = date,
                DayNumber = date.Day,
                IsCurrentMonth = date.Month == _currentMonth.Month,
                IsToday = date.Date == DateTime.Today,
                IsSelected = date.Date == SelectedDate.Date,
                HasSchedule = scheduledDates.Contains(date.ToString("yyyy-MM-dd"))
            });
        }
    }

    private async Task LoadSelectedDateAsync()
    {
        var schedules = await _scheduleService.GetForDateAsync(SelectedDate);

        SchedulesForSelectedDate.Clear();
        foreach (var s in schedules)
            SchedulesForSelectedDate.Add(s);

        OnPropertyChanged(nameof(HasSchedulesForSelectedDate));
        OnPropertyChanged(nameof(SchedulesCountLabel));
    }
}

/// <summary>Une case du calendrier mensuel.</summary>
public class CalendarDay : BaseViewModel
{
    public DateTime Date { get; set; }
    public int DayNumber { get; set; }
    public bool IsCurrentMonth { get; set; }
    public bool IsToday { get; set; }
    public bool HasSchedule { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}