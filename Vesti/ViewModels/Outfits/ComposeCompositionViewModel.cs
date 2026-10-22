using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Vesti.Infrastructure;
using Vesti.Models;

namespace Vesti.ViewModels.Outfits;

/// <summary>
/// Étape 2 du parcours Composer : aperçu de la composition et nom de la tenue.
/// L'utilisateur peut retirer ou remplacer des pièces.
/// </summary>
public class ComposeCompositionViewModel : BaseViewModel
{
    public ComposeCompositionViewModel()
    {
        Items = new ObservableCollection<ClothingItem>();

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        NextCommand = new Command(async () => await GoNextAsync());
        RemoveItemCommand = new Command<ClothingItem>(OnRemoveItem);
    }

    // ── Données de l'étape ───────────────────────────────────────────────
    public ObservableCollection<ClothingItem> Items { get; }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value ?? string.Empty);
    }

    public bool HasItems => Items.Count > 0;
    public string ItemCountLabel => Items.Count == 1
        ? "1 pièce"
        : $"{Items.Count} pièces";

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand BackCommand { get; }
    public ICommand NextCommand { get; }
    public ICommand RemoveItemCommand { get; }

    // ── Chargement depuis ComposeState ───────────────────────────────────
    public void Load()
    {
        Items.Clear();
        foreach (var item in ComposeState.Current.SelectedItems)
            Items.Add(item);

        // Pré-remplir le nom si déjà saisi
        Name = ComposeState.Current.Name ?? string.Empty;

        OnPropertyChanged(nameof(HasItems));
        OnPropertyChanged(nameof(ItemCountLabel));
    }

    // ── Actions ──────────────────────────────────────────────────────────
    private void OnRemoveItem(ClothingItem item)
    {
        Items.Remove(item);
        ComposeState.Current.SelectedItems.Remove(item);
        OnPropertyChanged(nameof(HasItems));
        OnPropertyChanged(nameof(ItemCountLabel));
    }

    private async Task GoNextAsync()
    {
        if (Items.Count == 0)
        {
            ErrorMessage = "Ajoutez au moins une pièce.";
            return;
        }

        ComposeState.Current.Name = string.IsNullOrWhiteSpace(Name) ? null : Name.Trim();
        await Shell.Current.GoToAsync(Routes.ComposeOccasion);
    }
}
