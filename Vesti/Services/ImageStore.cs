using Microsoft.Maui.Graphics.Platform;

namespace Vesti.Services;

/// <summary>
/// Implémentation du service de photos pour 
///
/// FLUX COMPLET pour une photo :
///   1. L'utilisateur choisit une source (galerie ou caméra).
///   2. MAUI retourne un FileResult (flux temporaire).
///   3. On lit le flux, on redimensionne à MAX_SIDE px max de côté.
///   4. On compresse en JPEG à JPEG_QUALITY %.
///   5. On enregistre dans le dossier privé Images/ avec un nom unique.
///   6. On retourne le chemin RELATIF (ex : "Images/clothing_abc123.jpg").
///
/// POURQUOI redimensionner ?
///   Une photo de téléphone fait souvent 3–5 Mo et 4000×3000 px.
///   Affichée dans une carte de 130 px de haut, inutile de conserver
///   cette résolution. On réduit à MAX_SIDE = 1024 px : résultat ~80–150 Ko,
///   qualité visuelle identique à l'écran.
///
/// EMPLACEMENT :
///   FileSystem.AppDataDirectory = dossier privé Android de l'app,
///   inaccessible aux autres applications.
/// </summary>
public class ImageStore : IImageStore
{
    private const int MaxSide = 1024;       // px max (largeur ou hauteur)
    private const int JpegQuality = 82;     // % de qualité JPEG

    private readonly string _imagesDir;

    public ImageStore()
    {
        _imagesDir = Path.Combine(FileSystem.AppDataDirectory, "Images");
        Directory.CreateDirectory(_imagesDir);
    }

    // ── API publique ─────────────────────────────────────────────────────

    public async Task<string?> PickFromGalleryAsync()
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(
                new MediaPickerOptions { Title = "Choisir une photo" });

            return await ProcessFileResultAsync(result);
        }
        catch (PermissionException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ImageStore.PickFromGallery] Permission refusée : {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ImageStore.PickFromGallery] {ex}");
            return null;
        }
    }

    public async Task<string?> CaptureFromCameraAsync()
    {
        try
        {
            // Étape 1 : vérifier / demander la permission caméra 
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.Camera>();

            if (status != PermissionStatus.Granted)
            {
                System.Diagnostics.Debug.WriteLine(
                    "[ImageStore.CaptureFromCamera] Permission caméra refusée par l'utilisateur.");
                return null;
            }

            // Étape 2 : vérifier que le matériel supporte la capture
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                System.Diagnostics.Debug.WriteLine(
                    "[ImageStore.CaptureFromCamera] IsCaptureSupported = false (pas de caméra détectée).");
                return null;
            }

            // Étape 3 : lancer la capture
            var result = await MediaPicker.Default.CapturePhotoAsync();

            if (result is null)
            {
                System.Diagnostics.Debug.WriteLine(
                    "[ImageStore.CaptureFromCamera] CapturePhotoAsync a retourné null (annulé par l'utilisateur).");
                return null;
            }

            return await ProcessFileResultAsync(result);
        }
        catch (PermissionException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ImageStore.CaptureFromCamera] PermissionException : {ex.Message}");
            return null;
        }
        catch (FeatureNotSupportedException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ImageStore.CaptureFromCamera] Fonctionnalité non supportée : {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ImageStore.CaptureFromCamera] Erreur inattendue : {ex}");
            return null;
        }
    }

    public string? GetFullPath(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return null;

        var fullPath = Path.Combine(FileSystem.AppDataDirectory, relativePath);
        return File.Exists(fullPath) ? fullPath : null;
    }

    public Task DeleteAsync(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return Task.CompletedTask;

        try
        {
            var fullPath = Path.Combine(FileSystem.AppDataDirectory, relativePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch (Exception ex)
        {
            // Un échec de suppression d'image ne doit jamais bloquer l'app.
            System.Diagnostics.Debug.WriteLine($"[ImageStore.Delete] {ex}");
        }

        return Task.CompletedTask;
    }

    // ── Traitement interne ───────────────────────────────────────────────

    private async Task<string?> ProcessFileResultAsync(FileResult? fileResult)
    {
        if (fileResult is null) return null;

        await using var sourceStream = await fileResult.OpenReadAsync();

        // Nom de fichier unique basé sur un GUID court
        var fileName = $"clothing_{Guid.NewGuid():N}.jpg";
        var destPath = Path.Combine(_imagesDir, fileName);

        // Redimensionnement + compression via Microsoft.Maui.Graphics
        var resized = await ResizeImageAsync(sourceStream);

        await File.WriteAllBytesAsync(destPath, resized);

        // On retourne le chemin RELATIF depuis AppDataDirectory
        return Path.Combine("Images", fileName);
    }

    /// <summary>
    /// Redimensionne l'image si elle dépasse MAX_SIDE px,
    /// puis encode en JPEG à JPEG_QUALITY %.
    /// Utilise Microsoft.Maui.Graphics qui est inclus avec .NET MAUI.
    /// </summary>
    private static async Task<byte[]> ResizeImageAsync(Stream sourceStream)
    {
        // Lire les octets source
        using var ms = new MemoryStream();
        await sourceStream.CopyToAsync(ms);
        var sourceBytes = ms.ToArray();

        try
        {
            // Décoder avec Microsoft.Maui.Graphics
            using var inputStream = new MemoryStream(sourceBytes);
            var image = PlatformImage.FromStream(inputStream);

            if (image is null)
                return sourceBytes;

            // Calculer les nouvelles dimensions en conservant le ratio
            double w = image.Width;
            double h = image.Height;

            if (w <= MaxSide && h <= MaxSide)
            {
                // Déjà assez petite : encoder quand même en JPEG pour la taille
                using var outMs = new MemoryStream();
                image.Save(outMs, ImageFormat.Jpeg, JpegQuality / 100f);
                return outMs.ToArray();
            }

            double ratio = Math.Min(MaxSide / w, MaxSide / h);
            int newW = (int)(w * ratio);
            int newH = (int)(h * ratio);

            var resized = image.Resize(newW, newH, ResizeMode.Fit);

            using var resultMs = new MemoryStream();
            resized.Save(resultMs, ImageFormat.Jpeg, JpegQuality / 100f);
            return resultMs.ToArray();
        }
        catch (Exception ex)
        {
            // Si le redimensionnement échoue, on retourne les octets originaux.
            System.Diagnostics.Debug.WriteLine($"[ImageStore.Resize] {ex}");
            return sourceBytes;
        }
    }
}
