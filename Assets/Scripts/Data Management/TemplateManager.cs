using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Mono.Cecil.Cil;

public class TemplateManager
{
    private static TemplateManager _instance;
    public static TemplateManager Instance => _instance ??= new TemplateManager();

    public Dictionary<string, Unit> UnitTemplates { get; private set; }
    public Dictionary<string, StatModifier> ModifierTemplates { get; private set; }

    private readonly string _unitsSavePath;
    private readonly string _modifierSavePath;

    public TemplateManager()
    {
        UnitTemplates = new Dictionary<string, Unit>();
        ModifierTemplates = new Dictionary<string, StatModifier>();

        _unitsSavePath = Path.Combine(Application.persistentDataPath, "unit_templates.json");
        _modifierSavePath = Path.Combine(Application.persistentDataPath, "modifier_templates.json");
    }
    /// <summary>
    /// Loads templates from JSON file to memory
    /// </summary>
    public void LoadTemplates()
    {
        if (File.Exists(_unitsSavePath))
        {
            string unitsJson = File.ReadAllText(_unitsSavePath);
            UnitTemplates = JsonConvert.DeserializeObject<Dictionary<string, Unit>>(unitsJson)
                ?? new Dictionary<string, Unit>();
            Debug.Log($"Loaded {UnitTemplates.Count} unit templates from {_unitsSavePath}");
        }
        else
        {
            Debug.LogWarning("Unit template files does not exist.");
        }

        if (File.Exists(_modifierSavePath))
        {
            string modifiersJson = File.ReadAllText(_modifierSavePath);
            ModifierTemplates = JsonConvert.DeserializeObject<Dictionary<string, StatModifier>>(modifiersJson)
                ?? new Dictionary<string, StatModifier>();
            Debug.Log($"Loaded {ModifierTemplates.Count} modifier templates from {_modifierSavePath}");
        }
    }
    /// <summary>
    /// Saves current dictionary state into JSON file
    /// </summary>
    public void SaveTemplates()
    {
        try
        {
            string unitsJson = JsonConvert.SerializeObject(UnitTemplates, Formatting.Indented);
            File.WriteAllText(_unitsSavePath, unitsJson);

            string modifiersJson = JsonConvert.SerializeObject(ModifierTemplates, Formatting.Indented);
            File.WriteAllText(_modifierSavePath, modifiersJson);

            Debug.Log("Succesfully saves templates to JSON file");
        }
        catch (Exception ex)
        {
            Debug.Log($"Error occured while saving templates: {ex.Message}");
        }
    }

    public void UpdateTemplatesFromCsv(List<Unit> importedUnits, List<StatModifier> importedModifiers)
    {
        UnitTemplates.Clear();
        ModifierTemplates.Clear();

        foreach(var unit in importedUnits)
        {
            if (!UnitTemplates.ContainsKey(unit.Id))
            {
                UnitTemplates.Add(unit.Id, unit);
            }
        }

        foreach (var modifier in importedModifiers)
        {
            if (!ModifierTemplates.ContainsKey(modifier.Id))
            {
                ModifierTemplates.Add(modifier.Id, modifier);
            }
        }

        SaveTemplates();
    }
}
