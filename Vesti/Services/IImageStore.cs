namespace Vesti.Services;

/// <summary>
/// Contrat du service de gestion des photos de vêtements.
///
/// POURQUOI une interface ?
/// Les ViewModels dépendent de cette interface, pas de l'implémentation.
/// Cela permettra plus tard d'utiliser une implémentation de test sans appareil.
///
/// PRINCIPE :
///   Les images NE SONT PAS stockées dans SQLite (pas de BLOB).(
///   Elles sont copiées dans le dossier PRIVÉ de l'application
///   (FileSystem.AppDataDirectory/Images/) et seul le CHEMIN RELATIF
///   est conservé dans ClothingItem.ImagePath.
/// </summary>
public interface IImageStore
{
    /// <summary>
    /// Permet à l'utilisateur de choisir une photo depuis la galerie.
    /// Retourne le chemin relatif de la copie privée, ou null si annulé.
    /// </summary>
    Task<string?> PickFromGalleryAsync();

    /// <summary>
    /// Permet à l'utilisateur de prendre une photo avec l'appareil photo.
    /// Retourne le chemin relatif de la copie privée, ou null si annulé.
    /// </summary>
    Task<string?> CaptureFromCameraAsync();

    /// <summary>
    /// Retourne le chemin absolu d'une image à partir de son chemin relatif.
    /// Retourne null si le fichier n'existe pas.
    /// </summary>
    string? GetFullPath(string? relativePath);

    /// <summary>
    /// Supprime le fichier image associé à un chemin relatif.
    /// Sans effet si le fichier n'existe pas.
    /// </summary>
    Task DeleteAsync(string? relativePath);
}
