using Vesti.ViewModels.Wishlist;

namespace Vesti.Views.Wishlist;

public partial class AddWishlistItemPage : ContentPage
{
    public AddWishlistItemPage(AddWishlistItemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}