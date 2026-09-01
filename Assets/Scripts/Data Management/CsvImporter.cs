using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CsvImporter
{
    /// <summary>
    /// Imports units from CSV file according to Excel template
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static List<Unit> ImportUnits(string filePath, char separator = ',')
    {
        var units = new List<Unit>();

        if (!File.Exists(filePath))
        {
            Debug.LogError($"[CsvImporter] Unit file not found in : {filePath}");
            return units;
        }

        try
        {
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] values = lines[i].Split(separator);

                if (values.Length < 13)
                {
                    Debug.LogWarning($"[CsvImporter] not enough columns in {i + 1}. Skipping row");
                    continue;
                }

                var unit = new Unit
                {
                    Id = values[0].Trim(),
                    Faction = values[1].Trim(),
                    UnitName = values[2].Trim(),
                    BaseAttack = ParseInt(values[3]),
                    BaseDefence = ParseInt(values[4]),
                    BaseMorale = ParseInt(values[5]),
                    BaseRangedAttack = ParseInt(values[6]),
                    BaseRangedDefence = ParseInt(values[7]),
                    BaseRange = ParseInt(values[8]),
                    Mobility = ParseInt(values[9]),
                    Durability = ParseInt(values[10]),
                    SoldierCount = ParseInt(values[11]),
                    StartingSoldierCount = ParseInt(values[12]),
                    TemplateSoldierCount = ParseInt(values[12])
                };

                unit.InitializeHp();

                units.Add(unit);
            }

            Debug.Log($"[CsvImporter] Succesfuly imported {units.Count} units from csv file");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CsvImporter] Error occured during unit import: {ex.Message}");
        }

        return units;
    }
    /// <summary>
    /// Loads modifiers from CSV file according to Excel template
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static List<StatModifier> ImportModifiers(string filePath, char separator = ',')
    {
        var modifiers = new List<StatModifier>();

        if (!File.Exists(filePath))
        {
            Debug.LogError($"[CsvImporter] modifiers not found in: {filePath}");
            return modifiers;
        }

        try
        {
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] values = lines[i].Split(separator);

                if (values.Length < 9)
                {
                    Debug.LogWarning($"[CsvImporter] Not enough columns in in {i + 1}. Skipping");
                    continue;
                }

                var modifier = new StatModifier
                {
                    Id = values[0].Trim(),
                    DisplayName = values[1].Trim(),
                    AttackBonus = ParseInt(values[2]),
                    DefenceBonus = ParseInt(values[3]),
                    RangedAttackBonus = ParseInt(values[4]),
                    RangedDefenceBonus = ParseInt(values[5]),
                    RangeBonus = ParseInt(values[6]),
                    MobilityBonus = ParseInt(values[7]),
                    MoraleBonus = float.TryParse(values[8].Trim(), out float moraleResult) ? moraleResult : 0f
                };

                modifiers.Add(modifier);
            }

            Debug.Log($"[CsvImporter] Succesfully importer {modifiers.Count} modifiers.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CsvImporer] Error occured while importing modifiers: {ex.Message}");
        }

        return modifiers;
    }

    private static int ParseInt(string value)
    {
        return int.TryParse(value.Trim(), out int result) ? result : 0;
    }
}
