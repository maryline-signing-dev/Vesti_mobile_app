using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vesti.Data;
using Vesti.Repositories;
using Vesti.Services;
using Vesti.ViewModels.Home;
using Vesti.ViewModels.Onboarding;
using Vesti.ViewModels.Profile;
using Vesti.Views.Home;
using Vesti.Views.Onboarding;
using Vesti.Views.Profile;

namespace Vesti;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                // ── UI : Inter ────────────────────────────────────────────
                fonts.AddFont("Inter_18pt-Regular.ttf", "Inter");
                fonts.AddFont("Inter_18pt-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("Inter_18pt-Bold.ttf", "InterBold");

                // ── Logo : DM Serif Display ────────────────────────────────
                fonts.AddFont("DMSerifDisplay-Regular.ttf", "DMSerifDisplay");
            });

        // Base de données
        builder.Services.AddSingleton<VestiDatabase>();

        // Repositories
        builder.Services.AddSingleton<ProfileRepository>();
        builder.Services.AddSingleton<SettingsRepository>();

        // Services
        builder.Services.AddSingleton<ProfileService>();

        // ViewModels
        builder.Services.AddTransient<OnboardingViewModel>();
        builder.Services.AddTransient<CreateProfileViewModel>(); // nouveau
        builder.Services.AddTransient<HomeViewModel>();

        // Pages
        builder.Services.AddTransient<OnboardingPage>();
        builder.Services.AddTransient<CreateProfilePage>();      // nouveau
        builder.Services.AddTransient<HomePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Initialisation SQLite sur le pool de threads (pas de deadlock)
        var db = app.Services.GetRequiredService<VestiDatabase>();
        Task.Run(() => db.InitialiseAsync()).GetAwaiter().GetResult();

        return app;
    }
}
