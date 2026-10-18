using Microsoft.Extensions.DependencyInjection;
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
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVestiServices(this IServiceCollection services)
    {
        // ── Base de données ───────────────────────────────────────────
        services.AddSingleton<VestiDatabase>();

        // ── Repositories (accès SQL) ──────────────────────────────────
        services.AddSingleton<ProfileRepository>();
        services.AddSingleton<SettingsRepository>();
        services.AddSingleton<ClothingRepository>();            // I2

        // ── Services métier ───────────────────────────────────────────
        services.AddSingleton<ProfileService>();
        services.AddSingleton<ClothingService>();               // I2

        // ── ViewModels ────────────────────────────────────────────────
        services.AddTransient<OnboardingViewModel>();
        services.AddTransient<CreateProfileViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<WardrobeViewModel>();             // I2
        services.AddTransient<AddItemViewModel>();              // I2
        services.AddTransient<ItemDetailViewModel>();           // I2
        services.AddTransient<EditItemViewModel>();             // I2

        // ── Pages ─────────────────────────────────────────────────────
        services.AddTransient<OnboardingPage>();
        services.AddTransient<CreateProfilePage>();
        services.AddTransient<HomePage>();
        services.AddTransient<WardrobePage>();                  // I2
        services.AddTransient<AddItemPage>();                   // I2
        services.AddTransient<ItemDetailPage>();                // I2
        services.AddTransient<EditItemPage>();                  // I2
        services.AddTransient<AgendaPage>();                    // squelette I0
        services.AddTransient<ProfilePage>();                   // squelette I0

        return services;
    }
}