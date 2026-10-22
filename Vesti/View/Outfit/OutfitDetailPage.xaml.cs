using Vesti.ViewModels.Outfits;

namespace Vesti.Views.Outfits;

public partial class OutfitDetailPage : ContentPage
{
    private readonly OutfitDetailViewModel _viewModel;

    public OutfitDetailPage(OutfitDetailViewModel viewModel)
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
