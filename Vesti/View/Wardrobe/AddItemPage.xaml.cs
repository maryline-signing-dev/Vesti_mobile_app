using Vesti.ViewModels.Wardrobe;

namespace Vesti.Views.Wardrobe;

public partial class AddItemPage : ContentPage
{
    public AddItemPage(AddItemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}