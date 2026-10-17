using Vesti.Models;
using Vesti.Repositories;

namespace Vesti.Services;

/// <summary>
/// Logique métier liée au profil.
///
/// Le ViewModel pose ses questions ici (ex : "est-ce le premier lancement ?").
/// Le service interroge les repositories et applique les règles — jamais le ViewModel.
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

    // ── Lecture ──────────────────────────────────────────────────────────

    /// <summary>
    /// Retourne vrai si l'onboarding n'a jamais été terminé
    /// (= premier lancement ou données effacées).
    /// </summary>
    public async Task<bool> IsFirstLaunchAsync()
        => !await _settingsRepo.GetBoolAsync(SettingKeys.OnboardingCompleted);

    /// <summary>Profil actuel ou null si pas encore créé.</summary>
    public async Task<LocalProfile?> GetProfileAsync()
        => await _profileRepo.GetAsync();

    // ── Écriture ─────────────────────────────────────────────────────────

    /// <summary>
    /// Crée le profil et marque l'onboarding comme terminé.
    /// Retourne un message d'erreur si la validation échoue, null sinon.
    /// </summary>
    public async Task<string?> CreateProfileAsync(string displayName, string? email)
    {
        // Validation : le nom est obligatoire (1–50 caractères)
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

        return null; // null = succès
    }

    /// <summary>
    /// Met à jour le nom affiché (utilisé en I8 — paramètres).
    /// </summary>
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
}
