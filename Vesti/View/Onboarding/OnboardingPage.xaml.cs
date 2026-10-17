using Vesti.ViewModels.Onboarding;

namespace Vesti.Views.Onboarding;

public partial class OnboardingPage : ContentPage
{
    public OnboardingPage(OnboardingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
