namespace Vesti.Views.Profile;

public partial class TermsPage : ContentPage
{
    public TermsPage() => InitializeComponent();

    private async void OnBackTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("..");
}
