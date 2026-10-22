using SQLite;

namespace Vesti.Models;

/// <summary>
/// Une tenue = association nommée de plusieurs pièces.
///
/// RÈGLES MÉTIER :
///   - Une tenue doit contenir au moins une pièce.
///   - Une tenue peut exister sans occasion (occasion = null).
///   - Une tenue n'est pas planifiée ici : la planification est en I5.
///   - Supprimer une tenue ne supprime pas les pièces.
/// </summary>
[Table("Outfit")]
public class Outfit
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>Occasion : "Cours", "Travail", "Sortie", "Sport", etc. Facultatif.</summary>
    [MaxLength(50)]
    public string? Occasion { get; set; }

    public bool IsFavorite { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Propriété calculée — non stockée. Chargée par le service.</summary>
    [Ignore]
    public List<ClothingItem> Items { get; set; } = new();

    [Ignore]
    public string DisplayName
        => string.IsNullOrWhiteSpace(Name)
            ? $"Tenue du {CreatedAt:d MMMM}"
            : Name;

    [Ignore]
    public string Subtitle
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(Occasion)) parts.Add(Occasion);
            parts.Add($"{Items.Count} pièce{(Items.Count > 1 ? "s" : "")}");
            return string.Join(" · ", parts);
        }
    }
}
