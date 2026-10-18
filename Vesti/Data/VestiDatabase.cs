using SQLite;
using Vesti.Models;

namespace Vesti.Data;

/// <summary>
/// Point d'entrée unique vers SQLite (une seule connexion partagée).
/// Le fichier vesti.db est dans le dossier PRIVÉ de l'application.
/// </summary>
public class VestiDatabase
{
    private readonly SQLiteAsyncConnection _db;

    public VestiDatabase()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "vesti.db");
        _db = new SQLiteAsyncConnection(dbPath,
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache);
    }

    public SQLiteAsyncConnection Connection => _db;

    /// <summary>
    /// Crée les tables absentes. Sans effet sur les tables existantes :
    /// les données déjà enregistrées sont conservées.
    /// </summary>
    public async Task InitialiseAsync()
    {
        // I1
        await _db.CreateTableAsync<LocalProfile>();
        await _db.CreateTableAsync<AppSetting>();

        // I2
        await _db.CreateTableAsync<ClothingItem>();

        // Les tables des incréments suivants s'ajoutent ici.
    }
}