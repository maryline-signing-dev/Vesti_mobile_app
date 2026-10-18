using Microsoft.Extensions.DependencyInjection;
using Vesti.Views.Agenda;
using Vesti.Views.Home;
using Vesti.Views.Profile;
using Vesti.Views.Wardrobe;

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

        // Routes secondaires — Dressing (I2)
        Routing.RegisterRoute("wardrobe/add", typeof(AddItemPage));
        Routing.RegisterRoute("wardrobe/detail", typeof(ItemDetailPage));
        Routing.RegisterRoute("wardrobe/edit", typeof(EditItemPage));

        // I4 : Routing.RegisterRoute("wardrobe/compose", typeof(ComposePage));
        // I5 : Routing.RegisterRoute("agenda/plan",      typeof(PlanOutfitPage));
    }
}
