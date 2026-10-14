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
        out TemplateMatch match)
    {
        Unit template = TemplateFinder.Find(
            templates, save.TemplateId, save.UnitName, save.FactionID, out match);

        if (template == null) return null;

        Unit unit = template.CloneUnit();

        // State stored in the save overrides template defaults
        unit.Faction = save.FactionID;
        unit.StartingSoldierCount = save.StartingSoldierCount;
        unit.SoldierCount = Mathf.Max(0, save.CurrentSoldierCount);
        unit.IsBroken = save.IsBroken;

        return unit;
    }
}
