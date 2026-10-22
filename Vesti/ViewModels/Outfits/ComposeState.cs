using Vesti.Models;

namespace Vesti.ViewModels.Outfits;

/// <summary>
/// État partagé entre les 3 étapes du parcours Composer.
///
/// POURQUOI un objet partagé ?
/// Les 4 pages du parcours (Sélection → Composition → Occasion → Enregistrée)
/// sont des pages distinctes dans le Shell. Elles ne peuvent pas se passer
/// de données facilement via les QueryProperty (qui ne transportent que des
/// types simples comme int ou string).
///
/// Ce singleton léger (réinitialisé à chaque début de parcours) contient
/// les données accumulées pendant les étapes.
///
/// CYCLE DE VIE :
///   Réinitialiser Current.Reset() au début de chaque nouveau parcours
///   (dans ComposeSelectionViewModel.OnAppearing).
/// </summary>
public class ComposeState
{
    // Singleton de session : un seul parcours actif à la fois.
    public static readonly ComposeState Current = new();

    /// <summary>Pièces sélectionnées à l'étape 1.</summary>
    public List<ClothingItem> SelectedItems { get; set; } = new();

    /// <summary>Nom saisi à l'étape 2.</summary>
    public string? Name { get; set; }

    /// <summary>Occasion choisie à l'étape 3 (null = sans occasion).</summary>
    public string? Occasion { get; set; }

    /// <summary>Tenue enregistrée — disponible après la sauvegarde.</summary>
    public Outfit? SavedOutfit { get; set; }

    public void Reset()
    {
        SelectedItems = new();
        Name = null;
        Occasion = null;
        SavedOutfit = null;
    }
}
