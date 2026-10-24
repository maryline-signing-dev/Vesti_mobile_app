using Vesti.Models;
using Vesti.Repositories;

namespace Vesti.Services;

public record SaveOutfitResult(Outfit? Outfit, string? Error);

/// <summary>
/// Logique métier des tenues.
///
/// RESPONSABILITÉS :
///   - Créer / modifier / supprimer une tenue avec ses liaisons.
///   - Charger les pièces d'une tenue (propriété Outfit.Items).
///   - Règle de suppression d'une pièce : retirer ses liaisons,
///     supprimer les tenues devenues vides.
///  - DeleteOutfitAsync supprime aussi les planifications de la tenue
///   (via un délégué injecté pour éviter une dépendance circulaire directe
///   avec ScheduleService, qui dépend déjà de OutfitService).
/// </summary>
public class OutfitService
{
    private readonly OutfitRepository _outfitRepository;
    private readonly ClothingRepository _clothingRepository;

    // Callback optionnel, câblé après la création des deux services pour
    // éviter une dépendance circulaire OutfitService ↔ ScheduleService.
    public Func<int, Task>? OnOutfitDeleted { get; set; }
    public OutfitService(OutfitRepository outfitRepository,
                         ClothingRepository clothingRepository)
    {
        _outfitRepository = outfitRepository;
        _clothingRepository = clothingRepository;
    }

    // ── Lecture ──────────────────────────────────────────────────────────

    /// <summary>Toutes les tenues avec leurs pièces chargées.</summary>
    public async Task<List<Outfit>> GetAllAsync()
    {
        var outfits = await _outfitRepository.GetAllAsync();
        foreach (var outfit in outfits)
            outfit.Items = await _outfitRepository
                .GetClothingItemsForOutfitAsync(outfit.Id);
        return outfits;
    }

    /// <summary>Une tenue par son Id, avec ses pièces.</summary>
    public async Task<Outfit?> GetByIdAsync(int id)
    {
        var outfit = await _outfitRepository.GetByIdAsync(id);
        if (outfit is null) return null;
        outfit.Items = await _outfitRepository
            .GetClothingItemsForOutfitAsync(outfit.Id);
        return outfit;
    }

    public async Task<int> CountAsync()
        => await _outfitRepository.CountAsync();

    // ── Création ─────────────────────────────────────────────────────────

    /// <summary>
    /// Crée une tenue avec les pièces sélectionnées.
    /// Règle : au moins une pièce requise.
    /// </summary>
    public async Task<SaveOutfitResult> CreateAsync(
        string? name,
        string? occasion,
        List<int> clothingItemIds)
    {
        if (clothingItemIds.Count == 0)
            return new SaveOutfitResult(null,
                "Sélectionnez au moins une pièce pour créer une tenue.");

        var outfit = new Outfit
        {
            Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim(),
            Occasion = string.IsNullOrWhiteSpace(occasion) ? null : occasion.Trim()
        };

        await _outfitRepository.InsertAsync(outfit);

        // Insérer les liaisons
        for (int i = 0; i < clothingItemIds.Count; i++)
        {
            await _outfitRepository.InsertItemLinkAsync(new OutfitItem
            {
                OutfitId = outfit.Id,
                ClothingItemId = clothingItemIds[i],
                DisplayOrder = i
            });
        }

        outfit.Items = await _outfitRepository
            .GetClothingItemsForOutfitAsync(outfit.Id);

        return new SaveOutfitResult(outfit, null);
    }

    // ── Modification ─────────────────────────────────────────────────────

    public async Task<string?> UpdateAsync(
        int outfitId,
        string? name,
        string? occasion,
        List<int> clothingItemIds)
    {
        if (clothingItemIds.Count == 0)
            return "Sélectionnez au moins une pièce.";

        var outfit = await _outfitRepository.GetByIdAsync(outfitId);
        if (outfit is null) return "Cette tenue est introuvable.";

        outfit.Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
        outfit.Occasion = string.IsNullOrWhiteSpace(occasion) ? null : occasion.Trim();

        await _outfitRepository.UpdateAsync(outfit);

        // Remplacer toutes les liaisons
        await _outfitRepository.DeleteItemLinksForOutfitAsync(outfitId);
        for (int i = 0; i < clothingItemIds.Count; i++)
        {
            await _outfitRepository.InsertItemLinkAsync(new OutfitItem
            {
                OutfitId = outfitId,
                ClothingItemId = clothingItemIds[i],
                DisplayOrder = i
            });
        }

        return null;
    }

    // ── Suppression ───────────────────────────────────────────────────────

    /// <summary>
    /// supprime d'abord les planifications de la tenue 
    /// (via le callback optionnel OnOutfitDeleted), puis les liaisons, 
    /// puis la tenue elle-même.
    /// 
    /// Retire la pièce de toutes ses tenues.
    /// Supprime les tenues qui deviennent vides.
    /// </summary>
    public async Task DeleteOutfitAsync(int outfitId)
    {
        if (OnOutfitDeleted is not null)
            await OnOutfitDeleted(outfitId);

        await _outfitRepository.DeleteAsync(outfitId);
    }

    public async Task OnClothingItemDeletedAsync(int clothingItemId)
    {
        var affectedOutfitIds = await _outfitRepository
            .DeleteItemLinksByClothingItemAsync(clothingItemId);

        foreach (var outfitId in affectedOutfitIds)
        {
            if (await _outfitRepository.IsOutfitEmptyAsync(outfitId))
                await DeleteOutfitAsync(outfitId);
        }
    }
}
