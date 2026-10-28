namespace Vesti.Views.Profile;

public partial class HelpPage : ContentPage
{
    public HelpPage() => InitializeComponent();

    private async void OnBackTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("..");
}
