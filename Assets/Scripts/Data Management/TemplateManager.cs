using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class TemplateManager
{
    private static TemplateManager _instance;
    public static TemplateManager Instance => _instance ??= new TemplateManager();

    public Dictionary<string, Unit> UnitTemplates { get; private set; }
    public Dictionary<string, StatModifier> ModifierTemplates { get; private set; }
    public Dictionary<string, FactionData> FactionTemplates { get; private set; }
    public Dictionary<string, UnitVisualData> VisualTemplates { get; private set; }

    private readonly string _unitsSavePath;
    private readonly string _modifierSavePath;
    private readonly string _factionSavePath;
    private readonly string _visualSavePath;

    public TemplateManager()
    {
        UnitTemplates = new Dictionary<string, Unit>();
        ModifierTemplates = new Dictionary<string, StatModifier>();

        FactionTemplates = new Dictionary<string, FactionData>();
        VisualTemplates = new Dictionary<string, UnitVisualData>();

        _unitsSavePath = Path.Combine(Application.persistentDataPath, "unit_templates.json");
        _modifierSavePath = Path.Combine(Application.persistentDataPath, "modifier_templates.json");

        _factionSavePath = Path.Combine(Application.persistentDataPath, "faction_templates.json");
        _visualSavePath = Path.Combine(Application.persistentDataPath, "visual_templates.json");
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

        if (File.Exists(_factionSavePath))
        {
            string factionsJson = File.ReadAllText(_factionSavePath);
            FactionTemplates = JsonConvert.DeserializeObject<Dictionary<string, FactionData>>(factionsJson)
                ?? new Dictionary<string, FactionData>();
            Debug.Log($"Loaded {FactionTemplates.Count} faction templates from {_factionSavePath}");
        }

        if (File.Exists(_visualSavePath))
        {
            string visualJson = File.ReadAllText(_visualSavePath);
            VisualTemplates = JsonConvert.DeserializeObject<Dictionary<string, UnitVisualData>>(visualJson)
                ?? new Dictionary<string, UnitVisualData>();
            Debug.Log($"Loaded {VisualTemplates.Count} visual templates from {_visualSavePath}");
        }

        foreach(var unitKvp in UnitTemplates)
        {
            string unitId = unitKvp.Key;
            Unit unit = unitKvp.Value;

            if (unit.TemplateSoldierCount == 0 && unit.SoldierCount > 0)
            {
                unit.TemplateSoldierCount = unit.SoldierCount;

                if (unit.StartingSoldierCount == 0)
                {
                    unit.StartingSoldierCount = unit.SoldierCount;
                }
            }

            if (VisualTemplates.TryGetValue(unitId, out UnitVisualData visualData))
            {
                unit.VisualData = visualData;
            }
            else
            {
                Debug.LogWarning($"[TemplateManager] Brak przypisanych danych wizualnych dla jednostki: {unit.UnitName}. Stworzono domyślny profil.");
                unit.VisualData = new UnitVisualData();
            }
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

            string factionsJson = JsonConvert.SerializeObject(FactionTemplates, Formatting.Indented);
            File.WriteAllText(_factionSavePath, factionsJson);

            string visualJson = JsonConvert.SerializeObject(VisualTemplates, Formatting.Indented);
            File.WriteAllText(_visualSavePath, visualJson);

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
