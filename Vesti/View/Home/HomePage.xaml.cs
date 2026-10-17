using Vesti.ViewModels.Home;

namespace Vesti.Views.Home;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    // OnAppearing est appelé à chaque fois que la page devient visible
    // (navigation retour, changement d'onglet…).
    // C'est le bon endroit pour rafraîchir les données.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
