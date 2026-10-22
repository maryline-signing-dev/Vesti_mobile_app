using Vesti.ViewModels.Outfits;

namespace Vesti.Views.Outfits;

public partial class ComposeCompositionPage : ContentPage
{
    private readonly ComposeCompositionViewModel _viewModel;

    public ComposeCompositionPage(ComposeCompositionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Load();
    }
}
