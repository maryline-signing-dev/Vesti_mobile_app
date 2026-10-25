using SQLite;

namespace Vesti.Models;

/// <summary>
/// Une pièce SOUHAITÉE, distincte du dressing (ClothingItem = possédée).
///
/// RÈGLE MÉTIER CLEF :
///   Un souhait ne compte jamais comme une pièce possédée. Seul
///   "Marquer comme acquise" crée une vraie fiche ClothingItem,
///   après confirmation explicite de l'utilisateur (jamais automatique).
///
/// AcquiredAt : null tant que non acquise. Une fois renseignée, l'item
///   sort de la liste "Ma wishlist" (cf. WishlistService.GetActiveAsync).
/// </summary>
[Table("WishlistItem")]
public class WishlistItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(100), NotNull]
    public string Name { get; set; } = string.Empty;

    [MaxLength(30), NotNull]
    public string Category { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Color { get; set; }

    /// <summary>Prix estimé en FCFA. Facultatif, purement informatif.</summary>
    public int? Price { get; set; }

    /// <summary>Lien vers la boutique/produit. Facultatif.</summary>
    [MaxLength(500)]
    public string? Link { get; set; }

    /// <summary>Référence libre (modèle, numéro...). Facultatif.</summary>
    [MaxLength(200)]
    public string? Reference { get; set; }

    [MaxLength(260)]
    public string? ImagePath { get; set; }

    /// <summary>Date d'acquisition — null tant que non acquise.</summary>
    public DateTime? AcquiredAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Ignore]
    public bool IsAcquired => AcquiredAt is not null;

    [Ignore]
    public string Subtitle
        => string.IsNullOrWhiteSpace(Color) ? Category : $"{Category} · {Color}";

    [Ignore]
    public string PriceLabel
        => Price is { } p ? $"{p:N0} FCFA".Replace(",", " ") : string.Empty;

    [Ignore]
    public string StatusLabel => IsAcquired ? "Acquise" : "À acquérir";
}