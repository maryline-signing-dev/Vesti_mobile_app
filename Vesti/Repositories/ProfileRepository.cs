using SQLite;
using Vesti.Data;
using Vesti.Models;

namespace Vesti.Repositories;

/// <summary>
/// Seule couche qui parle SQL pour le profil.
/// Le ViewModel et le service ne voient jamais de requêtes SQL directement.
///
/// RÈGLE MVVM : SQL ici, logique métier dans ProfileService,
///              affichage dans ProfileViewModel.
/// </summary>
public class ProfileRepository
{
    private readonly SQLiteAsyncConnection _db;

    public ProfileRepository(VestiDatabase database)
    {
        _db = database.Connection;
    }

    /// <summary>Retourne le profil existant, ou null si aucun n'a été créé.</summary>
    public async Task<LocalProfile?> GetAsync()
    {
        // On cherche toujours l'enregistrement d'Id = 1.
        return await _db.FindAsync<LocalProfile>(1);
    }

    /// <summary>Crée ou met à jour le profil (upsert).</summary>
    public async Task SaveAsync(LocalProfile profile)
    {
        profile.UpdatedAt = DateTime.UtcNow;

        // InsertOrReplaceAsync : insère si l'Id n'existe pas, remplace sinon.
        await _db.InsertOrReplaceAsync(profile);
    }
}
