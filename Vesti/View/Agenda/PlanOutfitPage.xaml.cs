using Vesti.ViewModels.Agenda;

namespace Vesti.Views.Agenda;

public partial class PlanOutfitPage : ContentPage
{
    private readonly PlanOutfitViewModel _viewModel;

    public PlanOutfitPage(PlanOutfitViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(PlanOutfitViewModel.SelectedOccasion))
                RefreshOccasionChips();
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshOccasionChips();
    }

    /// <summary>
    /// Colore la puce d'occasion sélectionnée. Même logique que
    /// ComposeOccasionPage.xaml.cs — voir ce fichier pour l'explication
    /// du choix de cette approche en code-behind.
    /// </summary>
    private void RefreshOccasionChips()
    {
        try
        {
            var primary = (Color)Application.Current!.Resources["ColorPrimary"];
            var primaryLight = (Color)Application.Current!.Resources["ColorPrimaryLight"];
            var surface = (Color)Application.Current!.Resources["ColorSurface"];
            var textPrimary = (Color)Application.Current!.Resources["ColorTextPrimary"];
            var selected = _viewModel.SelectedOccasion;

            if (Content is not ScrollView scroll) return;
            if (scroll.Content is not VerticalStackLayout vsl) return;

            FlexLayout? flex = null;
            foreach (var child in vsl.Children)
                if (child is FlexLayout f) { flex = f; break; }
            if (flex is null) return;

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
            System.Diagnostics.Debug.WriteLine($"[PlanOutfitPage] RefreshChips: {ex.Message}");
        }
    }
}