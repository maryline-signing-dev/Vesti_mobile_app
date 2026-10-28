namespace Vesti.Views.Profile;

public partial class PrivacyPage : ContentPage
{
    public PrivacyPage() => InitializeComponent();

    private async void OnBackTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("..");
}
