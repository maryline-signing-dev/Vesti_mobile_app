using Microsoft.Extensions.DependencyInjection;
using Vesti.Data;
using Vesti.Repositories;
using Vesti.Services;
using Vesti.ViewModels.Agenda;
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
/// Chaque incrément ajoute ses lignes ICI, jamais dans MauiProgram.cs.
/// MauiProgram.cs garde ainsi ses polices et son initialisation intactes.
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
        services.AddSingleton<OutfitScheduleRepository>();

        // ── Services métier ───────────────────────────────────────────
        services.AddSingleton<ProfileService>();
        services.AddSingleton<IImageStore, ImageStore>();       
        services.AddSingleton<OutfitService>();
        services.AddSingleton<ClothingService>();
        services.AddSingleton<ScheduleService>();

        // ── ViewModels ────────────────────────────────────────────────
        services.AddTransient<OnboardingViewModel>();
        services.AddTransient<CreateProfileViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<WardrobeViewModel>();
        services.AddTransient<AddItemViewModel>();
        services.AddTransient<ItemDetailViewModel>();
        services.AddTransient<EditItemViewModel>();
        services.AddTransient<OutfitListViewModel>();           
        services.AddTransient<ComposeSelectionViewModel>();     
        services.AddTransient<ComposeCompositionViewModel>();   
        services.AddTransient<ComposeOccasionViewModel>();      
        services.AddTransient<OutfitDetailViewModel>();
        services.AddTransient<AgendaViewModel>();               
        services.AddTransient<PlanOutfitViewModel>();           
        services.AddTransient<ScheduleDetailViewModel>();

        // ── Pages ─────────────────────────────────────────────────────
        services.AddTransient<OnboardingPage>();
        services.AddTransient<CreateProfilePage>();
        services.AddTransient<HomePage>();
        services.AddTransient<WardrobePage>();
        services.AddTransient<AddItemPage>();
        services.AddTransient<ItemDetailPage>();
        services.AddTransient<EditItemPage>();
        services.AddTransient<OutfitListPage>();                
        services.AddTransient<ComposeSelectionPage>();          
        services.AddTransient<ComposeCompositionPage>();        
        services.AddTransient<ComposeOccasionPage>();           
        services.AddTransient<OutfitDetailPage>();              
        services.AddTransient<AgendaPage>();
        services.AddTransient<PlanOutfitPage>();                
        services.AddTransient<ScheduleDetailPage>();
        services.AddTransient<ProfilePage>();

        return services;
    }
}
