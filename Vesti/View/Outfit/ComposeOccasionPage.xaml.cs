using Vesti.ViewModels.Outfits;

namespace Vesti.Views.Outfits;

public partial class ComposeOccasionPage : ContentPage
{
    private readonly ComposeOccasionViewModel _viewModel;

    public ComposeOccasionPage(ComposeOccasionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        // Écouter la sélection d'occasion pour colorier la puce active
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ComposeOccasionViewModel.SelectedOccasion))
                RefreshOccasionChips();
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Load();
        RefreshOccasionChips();
    }

    /// <summary>
    /// Met à jour la couleur de fond des puces d'occasion.
    /// La puce sélectionnée reçoit ColorPrimaryLight, les autres restent blanches.
    /// Cette logique est en code-behind car FlexLayout + BindableLayout
    /// ne supporte pas facilement le VisualState conditionnel sur une valeur externe.
    /// </summary>
    private void RefreshOccasionChips()
    {
        // La FlexLayout n'est pas nommée — on accède via le visual tree
        // Cette méthode est appelée uniquement pour la colorisation des puces.
        // Elle est tolérante : si la structure change, elle ne plante pas.
        try
        {
            var primary = (Color)Application.Current!.Resources["ColorPrimary"];
            var primaryLight = (Color)Application.Current!.Resources["ColorPrimaryLight"];
            var surface = (Color)Application.Current!.Resources["ColorSurface"];
            var textPrimary = (Color)Application.Current!.Resources["ColorTextPrimary"];
            var selected = _viewModel.SelectedOccasion;

            // Parcourir les enfants de la FlexLayout
            if (Content is not Grid grid) return;
            if (grid.Children.Count < 2) return;
            if (grid.Children[1] is not ScrollView scroll) return;
            if (scroll.Content is not VerticalStackLayout vsl) return;
            if (vsl.Children.Count < 2) return;
            if (vsl.Children[1] is not FlexLayout flex) return;

            foreach (var child in flex.Children)
            {
                if (child is not Border border) continue;
                if (border.Content is not Label label) continue;

                var isSelected = label.Text == selected;
                border.BackgroundColor = isSelected ? primaryLight : surface;
                label.TextColor = isSelected ? primary : textPrimary;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ComposeOccasionPage] RefreshChips: {ex.Message}");
        }
    }
}
