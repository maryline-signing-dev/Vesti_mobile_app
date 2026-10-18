using Vesti.ViewModels.Wardrobe;

namespace Vesti.Views.Wardrobe;

public partial class EditItemPage : ContentPage
{
    public EditItemPage(EditItemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}