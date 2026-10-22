using Vesti.ViewModels.Outfits;

namespace Vesti.Views.Outfits;

public partial class ComposeSelectionPage : ContentPage
{
    private readonly ComposeSelectionViewModel _viewModel;

    public ComposeSelectionPage(ComposeSelectionViewModel viewModel)
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
