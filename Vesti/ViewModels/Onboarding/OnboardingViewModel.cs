using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Vesti.Views.Profile;

namespace Vesti.ViewModels.Onboarding;

/// <summary>
/// Gère les 4 étapes d'onboarding :
/// 0 — Splash
/// 1 — Organiser
/// 2 — Composer
/// 3 — Planifier
///
/// À la dernière étape, l'utilisateur accède à CreateProfilePage.
/// La création du profil est gérée par CreateProfileViewModel.
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

    // ── Étapes ──────────────────────────────────────────────────────

    private int _currentStep;

    public int CurrentStep
    {
        get => _currentStep;
        set
        {
            if (SetProperty(ref _currentStep, value))
            {
                OnPropertyChanged(nameof(IsSplash));
                OnPropertyChanged(nameof(IsLastStep));
                OnPropertyChanged(nameof(ShowSkip));
                OnPropertyChanged(nameof(ShowFooter));
                OnPropertyChanged(nameof(ProgressWidth1));
                OnPropertyChanged(nameof(ProgressWidth2));
                OnPropertyChanged(nameof(ProgressWidth3));
            }
        }
    }

    public bool IsSplash => CurrentStep == 0;

    public bool IsLastStep => CurrentStep == 3;

    public bool ShowSkip => CurrentStep >= 1;

    public bool ShowFooter => CurrentStep >= 1;

    // ── Indicateurs de progression ──────────────────────────────────

    public double ProgressWidth1 => CurrentStep == 1 ? 24 : 8;

    public double ProgressWidth2 => CurrentStep == 2 ? 24 : 8;

    public double ProgressWidth3 => CurrentStep == 3 ? 24 : 8;

    // ── Commandes ───────────────────────────────────────────────────

    public ICommand NextCommand { get; }

    public ICommand SkipCommand { get; }

    public ICommand StartCommand { get; }

    // ── Navigation entre les étapes ─────────────────────────────────

    private void OnNext()
    {
        if (CurrentStep < 3)
        {
            CurrentStep++;
        }
    }

    private void OnSkip()
    {
        CurrentStep = 3;
    }

    // ── Accès à la création du profil ────────────────────────────────

    private async Task OnStartAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        try
        {
            var profilePage =
                _services.GetRequiredService<CreateProfilePage>();

            var navigation = Application.Current?.MainPage?.Navigation;

            if (navigation is null)
            {
                throw new InvalidOperationException(
                    "La navigation de l'application n'est pas disponible.");
            }

            await navigation.PushAsync(profilePage);
        }
        catch (Exception ex)
        {
            ErrorMessage =
                "Impossible d'ouvrir la création du profil. Réessayez.";

            System.Diagnostics.Debug.WriteLine(
                $"[OnboardingViewModel.OnStartAsync] {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}