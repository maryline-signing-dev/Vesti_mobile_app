using SQLite;

namespace Vesti.Models;

/// <summary>
/// Planification d'une tenue à une date donnée.
///
/// RÈGLES MÉTIER :
///   - Une tenue peut avoir plusieurs planifications (portée à des dates différentes).
///   - L'heure est facultative : si absente, la tenue est "prévue pour la journée".
///   - La planification n'est jamais créée automatiquement (toujours une action explicite).
///   - Supprimer une planification ne supprime pas la tenue.
///
/// STOCKAGE DES DATES :
///   ScheduledDate stocké en "yyyy-MM-dd" (tri alphabétique = tri chronologique).
///   ScheduledTime stocké en "HH:mm", null si pas d'heure précisée.
/// </summary>
[Table("OutfitSchedule")]
public class OutfitSchedule
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int OutfitId { get; set; }

    /// <summary>Date au format "yyyy-MM-dd".</summary>
    [Indexed, MaxLength(10), NotNull]
    public string ScheduledDate { get; set; } = string.Empty;

    /// <summary>Heure au format "HH:mm", ou null si non précisée.</summary>
    [MaxLength(5)]
    public string? ScheduledTime { get; set; }

    /// <summary>Occasion de ce rendez-vous (facultatif, reprend celle de la tenue par défaut).</summary>
    [MaxLength(50)]
    public string? Occasion { get; set; }

    /// <summary>
    /// Rappel activé — la fonctionnalité de notification n'est pas encore
    /// implémentée (reportée). Ce champ existe pour éviter une migration
    /// future, mais aucune notification n'est programmée pour l'instant.
    /// </summary>
    public bool ReminderEnabled { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // ── Propriétés calculées (non stockées) ──────────────────────────────

    [Ignore]
    public Outfit? Outfit { get; set; }

    [Ignore]
    public DateTime Date
        => DateTime.TryParse(ScheduledDate, out var d) ? d : DateTime.Today;

    [Ignore]
    public TimeSpan? Time
        => ScheduledTime is not null && TimeSpan.TryParse(ScheduledTime, out var t)
            ? t : null;

    [Ignore]
    public string TimeLabel
        => Time is { } t ? t.ToString(@"hh\:mm") : "Toute la journée";

    [Ignore]
    public string? ThumbnailImagePath
        => Outfit?.Items.FirstOrDefault()?.ImagePath;
}