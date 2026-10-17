using SQLite;
using Vesti.Models;

namespace Vesti.Data;

/// <summary>
/// Point d'entrée unique vers SQLite.
///
/// POURQUOI une seule instance ?
/// SQLite sur Android ne supporte pas bien les accès concurrents depuis
/// plusieurs connexions. On ouvre une connexion au démarrage et on la
/// réutilise partout (pattern "singleton via DI").
///
/// CHEMIN DE LA BASE :
/// FileSystem.AppDataDirectory → dossier privé de l'app sur Android,
/// inaccessible aux autres applications.
/// </summary>
public class VestiDatabase
{
    private readonly SQLiteAsyncConnection _db;

    // Version du schéma : à incrémenter à chaque migration future.
    private const int SchemaVersion = 1;

    public VestiDatabase()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "vesti.db");
        _db = new SQLiteAsyncConnection(dbPath,
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache);
    }

    /// <summary>
    /// Initialise la base : crée les tables si elles n'existent pas.
    /// Appelé une seule fois au démarrage depuis MauiProgram.cs.
    /// </summary>
    public async Task InitialiseAsync()
    {
        // CreateTableAsync est idempotent : sans effet si la table existe déjà.
        await _db.CreateTableAsync<LocalProfile>();
        await _db.CreateTableAsync<AppSetting>();

        // Les tables des incréments suivants seront ajoutées ici (I2, I4…).
    }

    // ── Exposition de la connexion aux repositories ──────────────────────
    // Les repositories reçoivent cette connexion via le constructeur (DI).
    // Ils ne créent jamais leur propre connexion.

    public SQLiteAsyncConnection Connection => _db;
}
