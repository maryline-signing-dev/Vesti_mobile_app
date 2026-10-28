using Vesti.ViewModels.Profile;

namespace Vesti.Views.Profile;

public partial class ProfilePage : ContentPage
{
    private readonly ProfilePageViewModel _viewModel;

    public ProfilePage(ProfilePageViewModel viewModel)
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
