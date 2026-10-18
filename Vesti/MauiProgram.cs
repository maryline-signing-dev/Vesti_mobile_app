using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vesti.Data;
using Vesti.Repositories;
using Vesti.Services;
using Vesti.ViewModels.Home;
using Vesti.ViewModels.Onboarding;
using Vesti.ViewModels.Profile;
using Vesti.ViewModels.Wardrobe;
using Vesti.Views.Agenda;
using Vesti.Views.Home;
using Vesti.Views.Onboarding;
using Vesti.Views.Profile;
using Vesti.Views.Wardrobe;

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
                fonts.AddFont("Inter-Regular.ttf", "Inter");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("Inter-Bold.ttf", "InterBold");
                fonts.AddFont("DMSerifDisplay-Regular.ttf", "DMSerifDisplay");
            });

        // ── Base de données ────────────────────────────────────────────
        builder.Services.AddSingleton<VestiDatabase>();

        // ── Repositories ──────────────────────────────────────────────
        builder.Services.AddSingleton<ProfileRepository>();
        builder.Services.AddSingleton<SettingsRepository>();
        builder.Services.AddSingleton<ClothingRepository>();

        // ── Services ──────────────────────────────────────────────────
        builder.Services.AddSingleton<ProfileService>();
        builder.Services.AddSingleton<ClothingService>();

        // ── ViewModels ────────────────────────────────────────────────
        builder.Services.AddTransient<OnboardingViewModel>();
        builder.Services.AddTransient<CreateProfileViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<WardrobeViewModel>();
        builder.Services.AddTransient<AddItemViewModel>();
        builder.Services.AddTransient<ItemDetailViewModel>();
        builder.Services.AddTransient<EditItemViewModel>();

        // ── Navigation principale ────────────────────────────────────────
        builder.Services.AddSingleton<AppShell>();

        // ── Pages ─────────────────────────────────────────────────────
        builder.Services.AddTransient<OnboardingPage>();
        builder.Services.AddTransient<CreateProfilePage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<WardrobePage>();
        builder.Services.AddTransient<AddItemPage>();
        builder.Services.AddTransient<ItemDetailPage>();
        builder.Services.AddTransient<EditItemPage>();

        // Pages squelettes des onglets non encore développés
        builder.Services.AddTransient<AgendaPage>();
        builder.Services.AddTransient<ProfilePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Initialisation SQLite sur le pool de threads (évite le deadlock)
        var db = app.Services.GetRequiredService<VestiDatabase>();
        Task.Run(() => db.InitialiseAsync()).GetAwaiter().GetResult();

        return app;
    }
}
