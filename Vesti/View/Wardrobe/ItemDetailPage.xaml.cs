using Vesti.ViewModels.Wardrobe;

namespace Vesti.Views.Wardrobe;

public partial class ItemDetailPage : ContentPage
{
    private readonly ItemDetailViewModel _viewModel;

    public ItemDetailPage(ItemDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    // Au retour de l'écran "Modifier la pièce", on relit la base
    // pour afficher les valeurs à jour.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}