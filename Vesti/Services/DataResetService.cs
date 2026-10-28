using Vesti.Data;

namespace Vesti.Services;

/// <summary>
/// Efface TOUTES les données locales : toutes les tables SQLite et tous les
/// fichiers du dossier Images. Action irréversible, déclenchée uniquement
/// depuis Paramètres après confirmation explicite de l'utilisateur.
/// Après effacement, le profil n'existe plus : l'application revient à
/// l'onboarding au prochain lancement (même comportement qu'un premier
/// lancement).
/// </summary>
public class DataResetService
{
    private readonly VestiDatabase _database;

    public DataResetService(VestiDatabase database)
    {
        _database = database;
    }

    public async Task ClearAllDataAsync()
    {
        await _database.ClearAllTablesAsync();

        try
        {
            var imagesDir = Path.Combine(FileSystem.AppDataDirectory, "Images");
            if (Directory.Exists(imagesDir))
            {
                foreach (var file in Directory.GetFiles(imagesDir))
                    File.Delete(file);
            }
        }
        catch (Exception ex)
        {
            // Un échec de suppression de fichiers ne doit jamais bloquer
            // l'effacement des données : les enregistrements sont déjà vidés.
            System.Diagnostics.Debug.WriteLine($"[DataResetService] {ex}");
        }
    }
}
