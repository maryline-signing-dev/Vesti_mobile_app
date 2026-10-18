using Vesti.ViewModels.Wardrobe;

namespace Vesti.Views.Wardrobe;

public partial class WardrobePage : ContentPage
{
    private readonly WardrobeViewModel _viewModel;

    // Un SEUL constructeur : celui qui reçoit le ViewModel par injection.
    // Ne pas ajouter de constructeur sans paramètre.
    public WardrobePage(WardrobeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    // Appelé à chaque fois que la page redevient visible
    // (changement d'onglet, retour depuis l'ajout, le détail ou la modification).
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}