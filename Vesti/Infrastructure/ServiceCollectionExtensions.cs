using Microsoft.Extensions.DependencyInjection;
using Vesti.Data;
using Vesti.Repositories;
using Vesti.Services;
using Vesti.ViewModels.Home;
using Vesti.ViewModels.Onboarding;
using Vesti.ViewModels.Outfits;
using Vesti.ViewModels.Profile;
using Vesti.ViewModels.Wardrobe;
using Vesti.Views.Agenda;
using Vesti.Views.Home;
using Vesti.Views.Onboarding;
using Vesti.Views.Outfits;
using Vesti.Views.Profile;
using Vesti.Views.Wardrobe;

namespace Vesti.Infrastructure;

/// <summary>
/// Enregistre TOUS les services de l'application dans le conteneur
/// d'injection de dépendances (DI).
///
/// Dans MauiProgram.cs, une seule ligne suffit :
///     builder.Services.AddVestiServices();
///
/// Chaque incrément ajoute ses lignes ICI, jamais dans MauiProgram.cs.
/// MauiProgram.cs garde ainsi ses polices et son initialisation intactes.
///
/// DURÉES DE VIE :
///   Singleton : une seule instance pour toute l'application
///               (base de données, repositories, services métier).
///   Transient : une nouvelle instance à chaque demande
///               (ViewModels et pages : un écran = un état neuf).
/// - Ajout OutfitRepository, OutfitService.
/// - Ajout ViewModels et pages du parcours Composer.
/// - ClothingService reçoit maintenant OutfitService 
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVestiServices(this IServiceCollection services)
    {
        // ── Base de données ───────────────────────────────────────────
        services.AddSingleton<VestiDatabase>();

        // ── Repositories ──────────────────────────────────────────────
        services.AddSingleton<ProfileRepository>();
        services.AddSingleton<SettingsRepository>();
        services.AddSingleton<ClothingRepository>();
        services.AddSingleton<OutfitRepository>();

        // ── Services métier ───────────────────────────────────────────
        services.AddSingleton<ProfileService>();
        services.AddSingleton<IImageStore, ImageStore>();       // I3
        services.AddSingleton<OutfitService>();
        services.AddSingleton<ClothingService>();

        // ── ViewModels ────────────────────────────────────────────────
        services.AddTransient<OnboardingViewModel>();
        services.AddTransient<CreateProfileViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<WardrobeViewModel>();
        services.AddTransient<AddItemViewModel>();
        services.AddTransient<ItemDetailViewModel>();
        services.AddTransient<EditItemViewModel>();
        services.AddTransient<OutfitListViewModel>();           // I4
        services.AddTransient<ComposeSelectionViewModel>();     // I4
        services.AddTransient<ComposeCompositionViewModel>();   // I4
        services.AddTransient<ComposeOccasionViewModel>();      // I4
        services.AddTransient<OutfitDetailViewModel>();

        // ── Pages ─────────────────────────────────────────────────────
        services.AddTransient<OnboardingPage>();
        services.AddTransient<CreateProfilePage>();
        services.AddTransient<HomePage>();
        services.AddTransient<WardrobePage>();
        services.AddTransient<AddItemPage>();
        services.AddTransient<ItemDetailPage>();
        services.AddTransient<EditItemPage>();
        services.AddTransient<OutfitListPage>();                // I4
        services.AddTransient<ComposeSelectionPage>();          // I4
        services.AddTransient<ComposeCompositionPage>();        // I4
        services.AddTransient<ComposeOccasionPage>();           // I4
        services.AddTransient<OutfitDetailPage>();              // I4
        services.AddTransient<AgendaPage>();
        services.AddTransient<ProfilePage>();

        return services;
    }
}
