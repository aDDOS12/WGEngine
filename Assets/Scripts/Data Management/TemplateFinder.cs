using System.Collections.Generic;
using System.Linq;

public enum TemplateMatch
{
    None,
    ById,
    ByNameAndFaction,
    ByNameOnly
}

/// <summary>
/// Finds the template a saved unit was created from.
/// Order: exact Id, then name + faction, then name only.
/// </summary>
public class TemplateFinder
{
    public static Unit Find(
        IReadOnlyDictionary<string, Unit> templates,
        string templateId,
        string unitName,
        string factionId,
        out TemplateMatch match)
    {
        // 1. Exact match by Id (saves made from this version on)
        if (!string.IsNullOrEmpty(templateId) && templates.TryGetValue(templateId, out Unit byId))
        {
            match = TemplateMatch.ById;
            return byId;
        }

        // 2. Name + faction (old saves, or the template was deleted)
        Unit byNameAndFaction = templates.Values.FirstOrDefault(
            t => t.UnitName == unitName && t.Faction == factionId);

        if (byNameAndFaction != null)
        {
            match = TemplateMatch.ByNameAndFaction;
            return byNameAndFaction;
        }

        // 3. Name only (template moved to another faction)
        Unit byName = templates.Values.FirstOrDefault(t => t.UnitName == unitName);
        match = byName != null ? TemplateMatch.ByNameOnly : TemplateMatch.None;
        return byName;
    }
}
