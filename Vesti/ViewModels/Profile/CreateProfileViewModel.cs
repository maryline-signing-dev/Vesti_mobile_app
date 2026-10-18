using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Vesti.Services;

namespace Vesti.ViewModels.Profile;

/// <summary>
/// ViewModel de l'écran "Votre dressing commence ici."
/// (création du profil local — appelé après l'onboarding).
///
/// Le terme retenu pour l'écran : "Créer mon profil".
/// Pas d'authentification distante. Toutes les données restent sur l'appareil.
/// </summary>
public class CreateProfileViewModel : BaseViewModel
{
    private readonly ProfileService _profileService;
    private readonly IServiceProvider _services;

    public CreateProfileViewModel(ProfileService profileService, IServiceProvider services)
    {
        _profileService = profileService;
        _services = services;
        SubmitCommand = new Command(async () => await SubmitAsync(), () => !IsBusy);
    }

    // ── Champs ───────────────────────────────────────────────────────────
    private string _displayName = string.Empty;
    public string DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    // ── Commandes ────────────────────────────────────────────────────────
    public ICommand SubmitCommand { get; }

    // ── Logique ──────────────────────────────────────────────────────────
    private async Task SubmitAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var error = await _profileService.CreateProfileAsync(DisplayName, Email);
            if (error is not null)
            {
                ErrorMessage = error;
                return;
            }

            // Profil créé : on remplace toute la navigation par le Shell principal
            Application.Current!.MainPage = _services.GetRequiredService<AppShell>();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Une erreur inattendue s'est produite. Réessayez.";
            System.Diagnostics.Debug.WriteLine($"[CreateProfileViewModel] {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
