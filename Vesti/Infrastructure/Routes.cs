namespace Vesti.Infrastructure;

/// <summary>
/// Noms de routes de navigation, centralisés au même endroit.
///
/// POURQUOI une classe dédiée ?
/// Les ViewModels n'ont pas à connaître les types de pages (Views) :
/// ils naviguent par nom de route, et AppShell associe le nom à la page.
/// </summary>
public static class Routes
{
    // Onglets (absolus, définis dans AppShell.xaml)
    public const string TabHome = "//home";
    public const string TabWardrobe = "//wardrobe";
    public const string TabAgenda = "//agenda";
    public const string TabProfile = "//profile";

    // Pages empilées au-dessus des onglets (enregistrées dans AppShell.xaml.cs)
    // Dressing 
    public const string AddItem = "add";
    public const string ItemDetail = "detail";
    public const string EditItem = "edit";

    // Tenues
    public const string OutfitList = "outfitlist";
    public const string ComposeSelection = "composeselection";
    public const string ComposeComposition = "composecomposition";
    public const string ComposeOccasion = "composeoccasion";
    public const string OutfitDetail = "outfitdetail";

    // Agenda 
    public const string PlanOutfit = "planoutfit";
    public const string ScheduleDetail = "scheduledetail";
}