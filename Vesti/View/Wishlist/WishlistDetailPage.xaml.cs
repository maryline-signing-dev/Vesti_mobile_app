using Vesti.ViewModels.Wishlist;

namespace Vesti.Views.Wishlist;

public partial class WishlistDetailPage : ContentPage
{
    public WishlistDetailPage(WishlistDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}