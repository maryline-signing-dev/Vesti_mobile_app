namespace Vesti.Models;

/// <summary>
/// Valeurs autorisées pour une pièce.
///
/// Les catégories viennent des maquettes (Haut, Bas, Veste, Robe, Chaussures)
/// complétées par Accessoire (présent dans la wishlist des maquettes).
/// La liste des couleurs est une PROPOSITION : elle reprend les couleurs vues
/// dans les maquettes (Blanc, Noir, Bleu, Beige, Marron) et ajoute les
/// couleurs courantes. À valider ou à ajuster.
///
/// Des tableaux (string[]) sont utilisés car le Picker de MAUI accepte
/// directement un IList.
/// </summary>
public static class ClothingOptions
{
    /// <summary>Valeur du filtre "toutes les catégories".</summary>
    public const string AllFilter = "Tout";

    public static readonly string[] Categories =
    {
        "Haut", "Bas", "Veste", "Robe", "Chaussures", "Accessoire"
    };

    public static readonly string[] Colors =
    {
        "Blanc", "Noir", "Gris", "Beige", "Marron", "Bleu",
        "Vert", "Rouge", "Rose", "Jaune", "Orange", "Violet", "Multicolore"
    };

    /// <summary>Puces de filtre : "Tout" puis chaque catégorie.</summary>
    public static readonly string[] FilterChips =
        new[] { AllFilter }.Concat(Categories).ToArray();

    /// <summary>
    /// Occasions disponibles pour une tenue.
    /// Conformes aux maquettes (Composer : Occasion).
    /// </summary>
    public static readonly string[] Occasions =
    {
        "Cours", "Travail", "Sortie", "Soirée",
        "Cérémonie", "Sport", "Quotidien", "Voyage", "Autre"
    };
}