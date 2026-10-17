using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Vesti.Views.Profile;

namespace Vesti.ViewModels.Onboarding;

/// <summary>
/// Gère les 4 étapes d'onboarding :
///   0 — Splash (logo seul, pas d'action)
///   1 — Organiser
///   2 — Composer
///   3 — Planifier
///
/// À l'étape 3, "Commencer" navigue vers CreateProfilePage
/// (création du profil local). Il n'y a aucun formulaire ici.
/// </summary>
public class OnboardingViewModel : BaseViewModel
{
    private readonly IServiceProvider _services;

    public OnboardingViewModel(IServiceProvider services)
    {
        _services = services;

        NextCommand = new Command(OnNext);
        SkipCommand = new Command(OnSkip);
        StartCommand = new Command(async () => await OnStartAsync());
    }

    // ── Étapes ──────────────────────────────────────────────────────────
    private int _currentStep = 0;
    public int CurrentStep
    {
        get => _currentStep;
        set
        {
            SetProperty(ref _currentStep, value);
            OnPropertyChanged(nameof(IsSplash));
            OnPropertyChanged(nameof(IsLastStep));
            OnPropertyChanged(nameof(ShowSkip));
            OnPropertyChanged(nameof(ShowFooter));
            OnPropertyChanged(nameof(ProgressWidth1));
            OnPropertyChanged(nameof(ProgressWidth2));
            OnPropertyChanged(nameof(ProgressWidth3));
        }
    }

    // Étape 0 = splash seul, pas de barre, pas de bouton
    public bool IsSplash => CurrentStep == 0;
    // Étape 3 = "Commencer" à la place de "Continuer"
    public bool IsLastStep => CurrentStep == 3;
    // "Passer" visible aux étapes 1, 2, 3 (pas sur le splash)
    public bool ShowSkip => CurrentStep >= 1;
    // Pied de page visible aux étapes 1, 2, 3
    public bool ShowFooter => CurrentStep >= 1;

    // Indicateurs de progression (étapes 1/2/3 uniquement)
    public double ProgressWidth1 => CurrentStep == 1 ? 24 : 8;
    public double ProgressWidth2 => CurrentStep == 2 ? 24 : 8;
    public double ProgressWidth3 => CurrentStep == 3 ? 24 : 8;

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand NextCommand { get; }
    public ICommand SkipCommand { get; }
    public ICommand StartCommand { get; }

    private void OnNext()
    {
        if (CurrentStep < 3) CurrentStep++;
    }

    private void OnSkip()
    {
        // "Passer" saute directement à la dernière étape
        CurrentStep = 3;
    }

    private async Task OnStartAsync()
    {
        // Navigation vers la création du profil local
        // (AppShell n'existe pas encore : navigation directe par MainPage)
        var profilePage = _services.GetRequiredService<CreateProfilePage>();
        await Application.Current!.MainPage!.Navigation.PushAsync(profilePage);
    }
}
