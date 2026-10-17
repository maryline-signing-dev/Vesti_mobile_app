using Vesti.ViewModels.Profile;

namespace Vesti.Views.Profile;

public partial class CreateProfilePage : ContentPage
{
    public CreateProfilePage(CreateProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
        => await Navigation.PopAsync();
}
