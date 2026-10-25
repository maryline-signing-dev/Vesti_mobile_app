using Microsoft.Extensions.DependencyInjection;
using Vesti.Views.Agenda;
using Vesti.Views.Home;
using Vesti.Views.Outfits;
using Vesti.Views.Profile;
using Vesti.Views.Wardrobe;
using Vesti.Views.Wishlist;
using Vesti.Views.Favorites;
using Vesti.Infrastructure;

namespace Vesti;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider services)
    {
        InitializeComponent();

        // Chaque page est résolue via le conteneur DI.
        // Cela garantit que les ViewModels sont correctement injectés.
        HomeTab.Content = services.GetRequiredService<HomePage>();
        WardrobeTab.Content = services.GetRequiredService<WardrobePage>();
        AgendaTab.Content = services.GetRequiredService<AgendaPage>();
        ProfileTab.Content = services.GetRequiredService<ProfilePage>();

        // Routes secondaires — Dressing 
        Routing.RegisterRoute(Routes.AddItem, typeof(AddItemPage));
        Routing.RegisterRoute(Routes.ItemDetail, typeof(ItemDetailPage));
        Routing.RegisterRoute(Routes.EditItem, typeof(EditItemPage));


        // Tenues 
        Routing.RegisterRoute(Routes.ComposeSelection, typeof(ComposeSelectionPage));
        Routing.RegisterRoute(Routes.ComposeComposition, typeof(ComposeCompositionPage));
        Routing.RegisterRoute(Routes.ComposeOccasion, typeof(ComposeOccasionPage));
        Routing.RegisterRoute(Routes.OutfitDetail, typeof(OutfitDetailPage));
        Routing.RegisterRoute(Routes.OutfitList, typeof(OutfitListPage));

        // Agenda 
        Routing.RegisterRoute(Routes.PlanOutfit, typeof(PlanOutfitPage));
        Routing.RegisterRoute(Routes.ScheduleDetail, typeof(ScheduleDetailPage));

        //Whistlist et Favoris
        Routing.RegisterRoute(Routes.WishlistList, typeof(WishlistListPage));
        Routing.RegisterRoute(Routes.AddWishlist, typeof(AddWishlistItemPage));
        Routing.RegisterRoute(Routes.WishlistDetail, typeof(WishlistDetailPage));
        Routing.RegisterRoute(Routes.Favorites, typeof(FavoritesPage));
    }
}
