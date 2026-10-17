using SQLite;

namespace Vesti.Models;

/// <summary>
/// Préférences techniques de l'application (clé / valeur texte).
/// Exemple de clés :
///   "OnboardingCompleted" → "true"
/// </summary>
[Table("AppSettings")]
public class AppSetting
{
    [PrimaryKey, MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Constantes pour les clés connues — évite les chaînes magiques.
/// </summary>
public static class SettingKeys
{
    public const string OnboardingCompleted = "OnboardingCompleted";
}
