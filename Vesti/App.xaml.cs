using Microsoft.Extensions.DependencyInjection;
using Vesti.Services;
using Vesti.Views.Onboarding;

namespace Vesti;

public partial class App : Application
{
    private readonly ProfileService _profileService;
    private readonly IServiceProvider _services;

    public App(ProfileService profileService, IServiceProvider services)
    {
        InitializeComponent();
        _profileService = profileService;
        _services = services;

        // Écran neutre pendant la lecture asynchrone (pas de deadlock)
        MainPage = new ContentPage
        {
            BackgroundColor = Color.FromArgb("#FAF9F7"),
            Content = new ActivityIndicator
            {
                IsRunning = true,
                Color = Color.FromArgb("#8C5A4A"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        };
    }

    protected override async void OnStart()
    {
        base.OnStart();
        try
        {
            var isFirstLaunch = await _profileService.IsFirstLaunchAsync();

            if (isFirstLaunch)
            {
                // OnboardingPage a besoin d'une NavigationPage
                // pour pouvoir faire PushAsync → CreateProfilePage
                var onboarding = _services.GetRequiredService<OnboardingPage>();
                MainPage = new NavigationPage(onboarding)
                {
                    // Masquer la barre native de NavigationPage
                    BarBackgroundColor = Color.FromArgb("#FAF9F7"),
                };
                NavigationPage.SetHasNavigationBar(onboarding, false);
            }
            else
            {
                MainPage = new AppShell();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[App.OnStart] {ex}");
            MainPage = new AppShell();
        }
    }
}
