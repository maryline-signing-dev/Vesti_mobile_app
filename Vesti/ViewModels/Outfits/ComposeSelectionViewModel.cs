using System.Collections.ObjectModel;
using System.Windows.Input;
using Vesti.Models;
using Vesti.Services;
using Vesti.Infrastructure;

namespace Vesti.ViewModels.Outfits;

/// <summary>
/// Étape 1 du parcours Composer : sélection des pièces.
/// L'utilisateur coche les pièces à inclure dans la tenue.
/// La sélection est transmise à l'étape suivante via ComposeState.
/// </summary>
public class ComposeSelectionViewModel : BaseViewModel
{
    private readonly ClothingService _clothingService;

    public ComposeSelectionViewModel(ClothingService clothingService)
    {
        _clothingService = clothingService;

        FilterChips = new ObservableCollection<string>(ClothingOptions.FilterChips);
        SelectableItems = new ObservableCollection<SelectableClothingItem>();

        SelectFilterCommand = new Command<string>(f => SelectedFilter = f);
        ToggleItemCommand = new Command<SelectableClothingItem>(OnToggle);
        ClearAllCommand = new Command(OnClearAll);
        NextCommand = new Command(
            async () => await GoNextAsync(),
            () => SelectedCount > 0);
    }

    // ── Collections ──────────────────────────────────────────────────────
    public ObservableCollection<SelectableClothingItem> SelectableItems { get; }
    public ObservableCollection<string> FilterChips { get; }

    // ── Filtre ───────────────────────────────────────────────────────────
    private string _selectedFilter = ClothingOptions.AllFilter;
    public string SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            if (SetProperty(ref _selectedFilter, value))
                _ = LoadAsync();
        }
    }

    // ── Compteur de sélection ────────────────────────────────────────────
    private int _selectedCount;
    public int SelectedCount
    {
        get => _selectedCount;
        private set
        {
            if (SetProperty(ref _selectedCount, value))
            {
                OnPropertyChanged(nameof(SelectionLabel));
                OnPropertyChanged(nameof(HasSelection));
                ((Command)NextCommand).ChangeCanExecute();
            }
        }
    }

    public bool HasSelection => _selectedCount > 0;
    public string SelectionLabel => _selectedCount == 1
        ? "1 pièce sélectionnée"
        : $"{_selectedCount} pièces sélectionnées";

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand SelectFilterCommand { get; }
    public ICommand ToggleItemCommand { get; }
    public ICommand ClearAllCommand { get; }
    public ICommand NextCommand { get; }

    // ── Chargement ───────────────────────────────────────────────────────
    public async Task LoadAsync()
    {
        try
        {
            var items = await _clothingService.GetItemsAsync(null, SelectedFilter);
            var selectedIds = SelectableItems
                .Where(s => s.IsSelected)
                .Select(s => s.Item.Id)
                .ToHashSet();

            SelectableItems.Clear();
            foreach (var item in items)
                SelectableItems.Add(new SelectableClothingItem(item)
                { IsSelected = selectedIds.Contains(item.Id) });

            RefreshCount();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger le dressing.";
            System.Diagnostics.Debug.WriteLine($"[ComposeSelectionViewModel] {ex}");
        }
    }

    // ── Actions ──────────────────────────────────────────────────────────
    private void OnToggle(SelectableClothingItem selectable)
    {
        selectable.IsSelected = !selectable.IsSelected;
        RefreshCount();
    }

    private void OnClearAll()
    {
        foreach (var s in SelectableItems)
            s.IsSelected = false;
        RefreshCount();
    }

    private void RefreshCount()
        => SelectedCount = SelectableItems.Count(s => s.IsSelected);

    private async Task GoNextAsync()
    {
        var selected = SelectableItems
            .Where(s => s.IsSelected)
            .Select(s => s.Item)
            .ToList();

        // Partage la sélection avec l'étape suivante via l'état partagé
        ComposeState.Current.SelectedItems = selected;

        await Shell.Current.GoToAsync(Routes.ComposeComposition);
    }
}

/// <summary>
/// Wrapper observable autour de ClothingItem pour gérer l'état de sélection.
/// </summary>
public class SelectableClothingItem : BaseViewModel
{
    public ClothingItem Item { get; }

    public SelectableClothingItem(ClothingItem item)
        => Item = item;

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
