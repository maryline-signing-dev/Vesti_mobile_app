using Vesti.ViewModels.Agenda;

namespace Vesti.Views.Agenda;

public partial class ScheduleDetailPage : ContentPage
{
    private readonly ScheduleDetailViewModel _viewModel;

    public ScheduleDetailPage(ScheduleDetailViewModel viewModel)
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
}