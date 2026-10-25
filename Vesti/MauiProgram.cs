using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vesti.Data;
using Vesti.Repositories;
using Vesti.Services;
using Vesti.ViewModels.Home;
using Vesti.ViewModels.Onboarding;
using Vesti.ViewModels.Profile;
using Vesti.ViewModels.Wardrobe;
using Vesti.ViewModels.Outfits;
using Vesti.ViewModels.Agenda;
using Vesti.ViewModels.Wishlist;
using Vesti.ViewModels.Favorites;
using Vesti.Views.Agenda;
using Vesti.Views.Home;
using Vesti.Views.Onboarding;
using Vesti.Views.Outfits;
using Vesti.Views.Profile;
using Vesti.Views.Wardrobe;
using Vesti.Views.Wishlist;
using Vesti.Views.Favorites;

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
        builder.Services.AddSingleton<OutfitRepository>();
        builder.Services.AddSingleton<OutfitScheduleRepository>();
        builder.Services.AddSingleton<WishlistRepository>();

        // ── Services ──────────────────────────────────────────────────
        builder.Services.AddSingleton<IImageStore, ImageStore>();
        builder.Services.AddSingleton<ProfileService>();
        builder.Services.AddSingleton<ClothingService>();
        builder.Services.AddSingleton<OutfitService>();
        builder.Services.AddSingleton<ScheduleService>();
        builder.Services.AddSingleton<WishlistService>();
        builder.Services.AddSingleton<FavoritesService>();

        // ── ViewModels ────────────────────────────────────────────────
        builder.Services.AddTransient<OnboardingViewModel>();
        builder.Services.AddTransient<CreateProfileViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<WardrobeViewModel>();
        builder.Services.AddTransient<AddItemViewModel>();
        builder.Services.AddTransient<ItemDetailViewModel>();
        builder.Services.AddTransient<EditItemViewModel>();
        builder.Services.AddTransient<ComposeCompositionViewModel>();
        builder.Services.AddTransient<ComposeOccasionViewModel>();
        builder.Services.AddTransient<ComposeSelectionViewModel>();
        builder.Services.AddTransient<OutfitListViewModel>();
        builder.Services.AddTransient<OutfitDetailViewModel>();
        builder.Services.AddTransient<ComposeState>();
        builder.Services.AddTransient<ScheduleDetailViewModel>();
        builder.Services.AddTransient<PlanOutfitViewModel>();
        builder.Services.AddTransient<AgendaViewModel>();
        builder.Services.AddTransient<AddWishlistItemViewModel>();
        builder.Services.AddTransient<WishlistDetailViewModel>();
        builder.Services.AddTransient<WishlistListViewModel>();
        builder.Services.AddTransient<FavoritesViewModel>();

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
        builder.Services.AddTransient<ComposeCompositionPage>();
        builder.Services.AddTransient<ComposeOccasionPage>();
        builder.Services.AddTransient<ComposeSelectionPage>();
        builder.Services.AddTransient<OutfitListPage>();
        builder.Services.AddTransient<OutfitDetailPage>();
        builder.Services.AddTransient<AgendaPage>();
        builder.Services.AddTransient<ScheduleDetailPage>();
        builder.Services.AddTransient<PlanOutfitPage>();
        builder.Services.AddTransient<AddWishlistItemPage>();
        builder.Services.AddTransient<WishlistDetailPage>();
        builder.Services.AddTransient<WishlistListPage>();
        builder.Services.AddTransient<FavoritesPage>();

        // Pages squelettes des onglets non encore développés
        builder.Services.AddTransient<ProfilePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Initialisation SQLite sur le pool de threads (évite le deadlock)
        var db = app.Services.GetRequiredService<VestiDatabase>();
        Task.Run(() => db.InitialiseAsync()).GetAwaiter().GetResult();

        // câblage du callback de suppression des planifications.
        // OutfitService et ScheduleService dépendent tous deux l'un de
        // l'autre indirectement (ScheduleService a besoin de OutfitService
        // pour charger les tenues ; OutfitService a besoin de prévenir
        // ScheduleService quand une tenue est supprimée). Pour éviter un
        // cycle d'injection de dépendances, le lien est fait ici, une fois
        // que les deux singletons existent.
        var outfitService = app.Services.GetRequiredService<OutfitService>();
        var scheduleService = app.Services.GetRequiredService<ScheduleService>();
        outfitService.OnOutfitDeleted = scheduleService.OnOutfitDeletedAsync;
        
        return app;
    }
}
