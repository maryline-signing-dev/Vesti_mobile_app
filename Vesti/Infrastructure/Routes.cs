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
    public const string HomeTab = "//home";
    public const string WardrobeTab = "//wardrobe";
    public const string AgendaTab = "//agenda";
    public const string ProfileTab = "//profile";

    // Pages empilées au-dessus des onglets
    // Dressing 
    public const string AddItem = "add";
    public const string ItemDetail = "detail";
    public const string EditItem = "edit";

    // Tenues
    public const string ComposeSelection = "composeselection";
    public const string ComposeComposition = "composecomposition";
    public const string ComposeOccasion = "composeoccasion";
    public const string OutfitDetail = "outfitdetail";
    public static string OutfitList = "outfitlist";

    // Agenda 
    public const string PlanOutfit = "planoutfit";
    public const string ScheduleDetail = "scheduledetail";

    // Wishlist 
    public const string WishlistList = "wishlistlist";
    public const string AddWishlist = "addwishlist";
    public const string WishlistDetail = "wishlistdetail";

    // Favoris 
    public const string Favorites = "favorites";

    // Statistiques
    public const string Statistics = "statistics";

    // Profile
    public const string EditProfile = "editprofile";
    public const string Settings = "settings";
    public const string Help = "help";
    public const string Terms = "terms";
    public const string Privacy = "privacy";

}