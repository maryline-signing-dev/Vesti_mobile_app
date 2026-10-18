using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Models;
using Vesti.Services;

namespace Vesti.ViewModels.Wardrobe;

/// <summary>
/// ViewModel de l'onglet Dressing.
///
/// ÉTATS D'ÉCRAN (un seul visible à la fois) :
///   IsEmpty    : aucune pièce du tout            → invitation à en ajouter une
///   IsNoResult : des pièces existent, mais ni la recherche ni le filtre
///                n'en retournent                  → message "aucun résultat"
///   HasItems   : au moins une pièce                → grille + recherche + filtres
///
/// Le XAML lie directement ces booléens : aucun convertisseur nécessaire.
/// </summary>
public class WardrobeViewModel : BaseViewModel
{
    private readonly ClothingService _clothingService;

    // Numéro de la dernière demande de chargement. Si l'utilisateur tape
    // plusieurs lettres vite, seul le résultat de la dernière demande est
    // affiché : les réponses "périmées" sont ignorées.
    private int _loadVersion;

    public WardrobeViewModel(ClothingService clothingService)
    {
        _clothingService = clothingService;

        AddItemCommand = new Command(async () =>
            await Shell.Current.GoToAsync(Routes.AddItem));

        ClearSearchCommand = new Command(() => SearchText = string.Empty);
    }

    // ── Données affichées ────────────────────────────────────────────────

    private IReadOnlyList<ClothingItem> _items = Array.Empty<ClothingItem>();
    public IReadOnlyList<ClothingItem> Items
    {
        get => _items;
        private set => SetProperty(ref _items, value);
    }

    /// <summary>Puces de filtre : "Tout", puis les catégories.</summary>
    public IList<string> Filters => ClothingOptions.FilterChips;

    // ── États d'écran ────────────────────────────────────────────────────

    private int _totalCount;

    public bool IsEmpty => _totalCount == 0;
    public bool HasItems => _totalCount > 0;
    public bool IsNoResult => HasItems && Items.Count == 0;
    public bool HasResults => Items.Count > 0;

    public bool IsSearchActive => !string.IsNullOrEmpty(SearchText);

    /// <summary>Titre au-dessus de la grille.</summary>
    public string ListTitle
    {
        get
        {
            if (!IsSearchActive)
                return "Toutes mes pièces";

            return Items.Count == 1 ? "1 résultat" : $"{Items.Count} résultats";
        }
    }

    // ── Recherche ────────────────────────────────────────────────────────

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value ?? string.Empty))
            {
                OnPropertyChanged(nameof(IsSearchActive));
                _ = LoadAsync();
            }
        }
    }

    // ── Filtre de catégorie ──────────────────────────────────────────────

    private string _selectedFilter = ClothingOptions.AllFilter;
    public string SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            // Le CollectionView peut envoyer null lors d'un rafraîchissement :
            // on ignore, pour garder toujours une puce sélectionnée.
            if (value is null) return;

            if (SetProperty(ref _selectedFilter, value))
                _ = LoadAsync();
        }
    }

    // ── Ouverture du détail d'une pièce ──────────────────────────────────

    private ClothingItem? _selectedItem;
    public ClothingItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value) && value is not null)
                _ = OpenItemAsync(value);
        }
    }

    private async Task OpenItemAsync(ClothingItem item)
    {
        try
        {
            await Shell.Current.GoToAsync($"{Routes.ItemDetail}?id={item.Id}");
        }
        finally
        {
            // On désélectionne : la même pièce pourra être rouverte au retour.
            SelectedItem = null;
        }
    }

    // ── Commandes ────────────────────────────────────────────────────────

    public ICommand AddItemCommand { get; }
    public ICommand ClearSearchCommand { get; }

    // ── Chargement ───────────────────────────────────────────────────────

    public async Task LoadAsync()
    {
        var version = ++_loadVersion;

        try
        {
            var total = await _clothingService.CountAsync();
            var items = await _clothingService.GetItemsAsync(SearchText, SelectedFilter);

            // Une demande plus récente est partie entre-temps : on ignore celle-ci.
            if (version != _loadVersion) return;

            _totalCount = total;
            Items = items;
            ErrorMessage = null;

            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(HasItems));
            OnPropertyChanged(nameof(IsNoResult));
            OnPropertyChanged(nameof(HasResults));
            OnPropertyChanged(nameof(ListTitle));
        }
        catch (Exception ex)
        {
            if (version == _loadVersion)
                ErrorMessage = "Impossible de charger votre dressing.";

            System.Diagnostics.Debug.WriteLine($"[WardrobeViewModel] {ex}");
        }
    }
}