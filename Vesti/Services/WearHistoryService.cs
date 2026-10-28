using Vesti.Models;
using Vesti.Repositories;

namespace Vesti.Services;

/// <summary>
/// Service de l'historique de port. Séparé de OutfitService pour garder
/// une responsabilité unique : OutfitService gère la tenue elle-même,
/// WearHistoryService gère le fait qu'elle a été portée.
/// </summary>
public class WearHistoryService
{
    private readonly WearHistoryRepository _repository;
    private readonly OutfitService _outfitService;

    public WearHistoryService(WearHistoryRepository repository, OutfitService outfitService)
    {
        _repository = repository;
        _outfitService = outfitService;
    }

    /// <summary>
    /// Enregistre que la tenue a été portée. Jamais appelé automatiquement.
    /// Retourne un message d'erreur si la tenue n'existe pas, sinon null.
    /// </summary>
    public async Task<string?> MarkWornAsync(int outfitId, DateTime? wornAt = null)
    {
        var outfit = await _outfitService.GetByIdAsync(outfitId);
        if (outfit is null) return "Cette tenue est introuvable.";

        await _repository.InsertAsync(new WearHistory
        {
            OutfitId = outfitId,
            OutfitNameSnapshot = outfit.DisplayName,
            WornAt = wornAt ?? DateTime.UtcNow
        });

        return null;
    }

    public async Task<int> CountAsync()
        => await _repository.CountAsync();

    public async Task<List<WearHistory>> GetAllAsync()
        => await _repository.GetAllAsync();
}
