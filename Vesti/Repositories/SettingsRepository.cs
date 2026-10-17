using SQLite;
using Vesti.Data;
using Vesti.Models;

namespace Vesti.Repositories;

/// <summary>
/// Accès aux préférences clé/valeur de l'application.
/// </summary>
public class SettingsRepository
{
    private readonly SQLiteAsyncConnection _db;

    public SettingsRepository(VestiDatabase database)
    {
        _db = database.Connection;
    }

    public async Task<string?> GetAsync(string key)
    {
        var row = await _db.FindAsync<AppSetting>(key);
        return row?.Value;
    }

    public async Task SetAsync(string key, string value)
    {
        await _db.InsertOrReplaceAsync(new AppSetting { Key = key, Value = value });
    }

    public async Task<bool> GetBoolAsync(string key, bool defaultValue = false)
    {
        var raw = await GetAsync(key);
        if (raw is null) return defaultValue;
        return raw == "true";
    }

    public async Task SetBoolAsync(string key, bool value)
        => await SetAsync(key, value ? "true" : "false");
}
