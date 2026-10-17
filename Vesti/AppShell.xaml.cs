namespace Vesti;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Les routes secondaires seront ajoutées ici à chaque incrément.
        // Exemple (I2) :
        // Routing.RegisterRoute("wardrobe/detail", typeof(Views.Wardrobe.ItemDetailPage));
        // Routing.RegisterRoute("wardrobe/add",    typeof(Views.Wardrobe.AddItemPage));
    }
}
