using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager
{
    private static DataManager _instance;
    public static DataManager Instance => _instance ??= new DataManager();
    public List<Unit> ActiveUnits { get; private set; } = new List<Unit>();
    public BattleConfiguration CurrentBattleConfig { get; set; }

    private DataManager()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        TemplateManager.Instance.LoadTemplates();

        Debug.Log($"[DataManager] initialized succesfuly. " +
            $"{TemplateManager.Instance.UnitTemplates.Count} unit templates and " +
            $"{TemplateManager.Instance.ModifierTemplates.Count} modifier templates loaded.");
    }

    public void ImportDataFromCsv(string unitsCsvPath, string modifiersCsvPath)
    {
        var importedUnits = CsvImporter.ImportUnits(unitsCsvPath);
        var importedModifiers = CsvImporter.ImportModifiers(modifiersCsvPath);

        TemplateManager.Instance.UpdateTemplatesFromCsv(importedUnits, importedModifiers);

        Debug.Log("[DataManager] Database was updated based on CSV files.");
    }

    public void SpawnUnitFromTemplate(string templateId)
    {
        if (TemplateManager.Instance.UnitTemplates.TryGetValue(templateId, out Unit template))
        {
            Unit activeInstance = template.CloneUnit();

            ActiveUnits.Add(activeInstance);

            Debug.Log($"[DataManager] Cloned template '{template.UnitName}'. " +
                $"Added instance to active with new ID: {activeInstance.Id}");
        }
        else
        {
            Debug.LogWarning($"[DataManager] Did not found unit template with ID: {templateId}");
        }
    }

    public void ApplyModifierToActiveUnit (string unitInstanceId, string modifierTemplateId)
    {
        Unit unit = ActiveUnits.Find(u => u.Id == unitInstanceId);
        if (unit == null)
        {
            Debug.LogWarning($"[DataManager] Nie znaleziono aktywnej jednostki o ID: {unitInstanceId}");
            return;
        }

        if (TemplateManager.Instance.ModifierTemplates.TryGetValue(modifierTemplateId, out StatModifier modifierTemplate))
        {
            unit.AddModifier(modifierTemplate);
            Debug.Log($"[DataManager] Applied modifier '{modifierTemplate.DisplayName}' to unit '{unit.UnitName}'.");
        }
        else
        {
            Debug.LogWarning($"[DataManager] Did not found modifier template with ID: {modifierTemplateId}");
        }
    }
}
