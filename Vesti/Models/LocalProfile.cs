using SQLite;

namespace Vesti.Models;

/// <summary>
/// Profil local de l'utilisateur.
/// Une seule ligne existera toujours dans la table (Id fixe = 1).
/// Aucun compte distant — tout reste sur l'appareil.
/// </summary>
[Table("LocalProfile")]
public class LocalProfile
{
    [PrimaryKey]
    public int Id { get; set; } = 1; // toujours 1 : un seul profil

    [MaxLength(50), NotNull]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// E-mail facultatif et purement informatif (pas d'authentification).
    /// </summary>
    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
