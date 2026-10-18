using SQLite;

namespace Vesti.Models;

/// <summary>
/// Une pièce de vêtement POSSÉDÉE par l'utilisateur (table ClothingItem).
/// Une pièce souhaitée (wishlist) sera une autre entité, en I6.
///
/// Obligatoires : Name, Category.
/// Facultatifs  : Color, Description, Season, Style, Brand, ImagePath.
/// ImagePath    : chemin RELATIF dans le stockage privé de l'app (I3).
///                Les photos ne sont jamais stockées dans la base.
/// </summary>
[Table("ClothingItem")]
public class ClothingItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(100), NotNull]
    public string Name { get; set; } = string.Empty;

    [MaxLength(30), NotNull]
    public string Category { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Color { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Season { get; set; }

    [MaxLength(50)]
    public string? Style { get; set; }

    [MaxLength(100)]
    public string? Brand { get; set; }

    [MaxLength(260)]
    public string? ImagePath { get; set; }

    public bool IsFavorite { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Texte "Catégorie · Couleur" affiché sous le nom.
    /// [Ignore] : propriété calculée, pas de colonne en base.
    /// </summary>
    [Ignore]
    public string Subtitle
        => string.IsNullOrWhiteSpace(Color) ? Category : $"{Category} · {Color}";
}