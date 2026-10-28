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
        await _db.CreateTableAsync<LocalProfile>();
        await _db.CreateTableAsync<AppSetting>();
        await _db.CreateTableAsync<ClothingItem>();
        await _db.CreateTableAsync<Outfit>();
        await _db.CreateTableAsync<OutfitItem>();
        await _db.CreateTableAsync<OutfitSchedule>();
        await _db.CreateTableAsync<WishlistItem>();
        await _db.CreateTableAsync<WearHistory>();
    }

    /// <summary>
    /// vide toutes les tables (pour l'effacement des données) sans
    /// supprimer le fichier de base lui-même — le schéma reste en place.
    /// </summary>
    public async Task ClearAllTablesAsync()
    {
        await _db.DeleteAllAsync<WearHistory>();
        await _db.DeleteAllAsync<WishlistItem>();
        await _db.DeleteAllAsync<OutfitSchedule>();
        await _db.DeleteAllAsync<OutfitItem>();
        await _db.DeleteAllAsync<Outfit>();
        await _db.DeleteAllAsync<ClothingItem>();
        await _db.DeleteAllAsync<AppSetting>();
        await _db.DeleteAllAsync<LocalProfile>();
    }
}