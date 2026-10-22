using Vesti.ViewModels.Outfits;

namespace Vesti.Views.Outfits;

public partial class OutfitListPage : ContentPage
{
    private readonly OutfitListViewModel _viewModel;

    public OutfitListPage(OutfitListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
