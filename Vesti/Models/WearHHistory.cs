using SQLite;

namespace Vesti.Models;

/// <summary>
/// Un événement "cette tenue a été portée à cette date".
///
/// RÈGLE MÉTIER CLEF :
///   Jamais créé automatiquement — uniquement via l'action explicite
///   "Marquer comme portée". C'est la source unique des statistiques
///   d'usage (tenues les plus/moins portées).
///
/// OutfitNameSnapshot : le nom de la tenue au moment où elle a été portée,
///   conservé même si la tenue est supprimée plus tard (I4 : la suppression
///   d'une tenue ne supprime pas son historique).
/// </summary>
[Table("WearHistory")]
public class WearHistory
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int OutfitId { get; set; }

    [MaxLength(100), NotNull]
    public string OutfitNameSnapshot { get; set; } = string.Empty;

    public DateTime WornAt { get; set; } = DateTime.UtcNow;

    [MaxLength(200)]
    public string? Note { get; set; }
}
