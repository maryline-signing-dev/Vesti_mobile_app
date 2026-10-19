namespace Vesti.Infrastructure;

/// <summary>
/// Noms de routes de navigation, centralisés au même endroit.
///
/// POURQUOI des noms plats (sans "/") ?
/// Une route comme "wardrobe/add" peut entrer en conflit avec la route de
/// l'onglet "wardrobe". Des noms uniques évitent toute ambiguïté dans Shell.
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

    // Pages empilées au-dessus des onglets (enregistrées dans AppShell.xaml.cs)
    public const string AddItem = "add";
    public const string ItemDetail = "detail";
    public const string EditItem = "edit";
}