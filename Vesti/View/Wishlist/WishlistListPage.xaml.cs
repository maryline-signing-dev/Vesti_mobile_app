using Vesti.ViewModels.Wishlist;

namespace Vesti.Views.Wishlist;

public partial class WishlistListPage : ContentPage
{
    private readonly WishlistListViewModel _viewModel;

    public WishlistListPage(WishlistListViewModel viewModel)
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

    private async void OnBackTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("..");
}