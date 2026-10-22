using SQLite;

namespace Vesti.Models;

/// <summary>
/// Table de liaison entre une tenue et ses pièces.
///
/// POURQUOI une table séparée ?
/// Une pièce peut apparaître dans plusieurs tenues.
/// Une tenue contient plusieurs pièces.
/// C'est une relation plusieurs-à-plusieurs (N:N).
/// SQLite ne gère pas directement les tableaux : on crée une table
/// intermédiaire avec une ligne par paire (OutfitId, ClothingItemId).
///
/// RÈGLE DE SUPPRESSION :
///   - Supprimer une tenue → supprimer toutes ses lignes OutfitItem.
///   - Supprimer une pièce → supprimer ses lignes OutfitItem dans toutes
///     les tenues. Si une tenue se retrouve sans pièce, elle est supprimée.
/// </summary>
[Table("OutfitItem")]
public class OutfitItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int OutfitId { get; set; }

    [Indexed]
    public int ClothingItemId { get; set; }

    /// <summary>Ordre d'affichage dans la composition.</summary>
    public int DisplayOrder { get; set; }
}
