namespace Vesti.ViewModels.Agenda;

/// <summary>
/// État partagé minimal : indique si la sélection d'une tenue depuis
/// "Mes tenues" doit mener à la planification (Agenda → "+") plutôt
/// qu'au détail habituel de la tenue.
///
/// FLUX :
///   1. Depuis Agenda, "Planifier une tenue" met IsSelectingForSchedule = true
///      puis navigue vers OutfitListPage.
///   2. OutfitListViewModel.SelectOutfitCommand vérifie ce drapeau :
///      s'il est actif, il navigue vers PlanOutfit au lieu de OutfitDetail,
///      puis réinitialise le drapeau.
///   3. Depuis le détail d'une tenue, "Ajouter à l'agenda" navigue
///      directement vers PlanOutfit (le drapeau n'est pas utilisé dans ce cas).
/// </summary>
public static class AgendaPlanState
{
    public static bool IsSelectingForSchedule { get; set; } = false;

    /// <summary>Date présélectionnée si on arrive depuis un jour du calendrier.</summary>
    public static DateTime? PreselectedDate { get; set; }
}