using Vesti.Models;
using Vesti.Repositories;

namespace Vesti.Services;

/// <summary>
/// MODIFIÉ en I8 : ajout de UpdateProfileAsync (nom + e-mail en une fois,
/// utilisé par l'écran "Modifier mes informations"). UpdateDisplayNameAsync
/// est conservée pour compatibilité avec le code existant.
/// Tout le reste est inchangé depuis I1.
/// </summary>
public class ProfileService
{
    private readonly ProfileRepository _profileRepo;
    private readonly SettingsRepository _settingsRepo;

    public ProfileService(ProfileRepository profileRepo, SettingsRepository settingsRepo)
    {
        _profileRepo = profileRepo;
        _settingsRepo = settingsRepo;
    }

    public async Task<bool> IsFirstLaunchAsync()
        => !await _settingsRepo.GetBoolAsync(SettingKeys.OnboardingCompleted);

    public async Task<LocalProfile?> GetProfileAsync()
        => await _profileRepo.GetAsync();

    public async Task<string?> CreateProfileAsync(string displayName, string? email)
    {
        displayName = displayName.Trim();
        if (string.IsNullOrEmpty(displayName))
            return "Le prénom est obligatoire.";
        if (displayName.Length > 50)
            return "Le prénom ne peut pas dépasser 50 caractères.";

        var profile = new LocalProfile
        {
            Id = 1,
            DisplayName = displayName,
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _profileRepo.SaveAsync(profile);
        await _settingsRepo.SetBoolAsync(SettingKeys.OnboardingCompleted, true);

        return null;
    }

    public async Task<string?> UpdateDisplayNameAsync(string displayName)
    {
        displayName = displayName.Trim();
        if (string.IsNullOrEmpty(displayName))
            return "Le prénom est obligatoire.";
        if (displayName.Length > 50)
            return "Le prénom ne peut pas dépasser 50 caractères.";

        var existing = await _profileRepo.GetAsync();
        if (existing is null) return "Profil introuvable.";

        existing.DisplayName = displayName;
        await _profileRepo.SaveAsync(existing);
        return null;
    }

    /// <summary>I8 : met à jour le nom et l'e-mail en une seule opération.</summary>
    public async Task<string?> UpdateProfileAsync(string displayName, string? email)
    {
        displayName = displayName.Trim();
        if (string.IsNullOrEmpty(displayName))
            return "Le prénom est obligatoire.";
        if (displayName.Length > 50)
            return "Le prénom ne peut pas dépasser 50 caractères.";

        var existing = await _profileRepo.GetAsync();
        if (existing is null) return "Profil introuvable.";

        existing.DisplayName = displayName;
        existing.Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        await _profileRepo.SaveAsync(existing);
        return null;
    }
}
