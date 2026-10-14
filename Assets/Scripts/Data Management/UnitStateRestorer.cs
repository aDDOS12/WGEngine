using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rebuilds a runtime unit from saved battle data.
/// Pure logic: no scene objects, no logging.
/// </summary>
public class UnitStateRestorer
{
    public static Unit Restore(
        UnitSaveData save,
        IReadOnlyDictionary<string, Unit> templates,
        out TemplateMatch match,
        IReadOnlyDictionary<string, StatModifier> modifierTemplates = null)
    {
        Unit template = TemplateFinder.Find(
            templates, save.TemplateId, save.UnitName, save.FactionID, out match);

        if (template == null) return null;

        Unit unit = template.CloneUnit();

        // State stored in the save overrides template defaults
        unit.Faction = save.FactionID;
        unit.StartingSoldierCount = save.StartingSoldierCount;
        unit.SoldierCount = Mathf.Max(0, save.CurrentSoldierCount);
        // HP is not stored in the save. Rebuild it from the soldier count
        // (decision in #28: accepts losing damage below one soldier).
        unit.InitializeHp();
        unit.IsBroken = save.IsBroken;
        RestoreModifiers(unit, save.Modifiers, modifierTemplates);

        return unit;
    }

    private static void RestoreModifiers(
        Unit unit,
        List<ModifierSaveData> saved,
        IReadOnlyDictionary<string, StatModifier> modifierTemplates)
    {
        if (saved == null) return; // Saves made before modifiers were stored

        foreach (ModifierSaveData data in saved)
        {
            // Prefer the current template. Fall back to the saved snapshot:
            // manual overrides and deleted templates have no template.
            StatModifier template = null;
            if (modifierTemplates != null && !string.IsNullOrEmpty(data.Id))
            {
                modifierTemplates.TryGetValue(data.Id, out template);
            }

            unit.AddModifier(template ?? data.ToModifier());
        }
    }
}
