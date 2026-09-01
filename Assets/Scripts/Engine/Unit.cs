using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : IModifiable
{
    public string Id { get; set; }
    public string Faction { get; set; }
    public string UnitName { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefence { get; set; }
    public int BaseMorale { get; set; }
    public int BaseRangedAttack { get; set; }
    public int BaseRangedDefence { get; set; }
    public int BaseRange { get; set; }
    public int Mobility { get; set; }
    public int Durability { get; set; }

    public int TemplateSoldierCount { get; set; }
    public int SoldierCount { get; set; }
    public int StartingSoldierCount { get; set; }


    public int CurrentHp { get; set; }
    public bool IsBroken { get; set; }
    public UnitVisualData VisualData { get; set; }

    public List<StatModifier> ActiveModifiers { get; private set; } = new List<StatModifier>();

    public int Attack => BaseAttack + ActiveModifiers.Sum(m => m.AttackBonus);
    public int Defence => BaseDefence + ActiveModifiers.Sum(m => m.DefenceBonus);
    public int RangedAttack => Mathf.Max(0, BaseRangedAttack + ActiveModifiers.Sum(m => m.RangedAttackBonus));
    public int RangedDefence => Mathf.Max(0, BaseRangedDefence + ActiveModifiers.Sum(m => m.RangedDefenceBonus));
    public float Morale => BaseMorale + ActiveModifiers.Sum(m => m.MoraleBonus);
    public int Range => BaseRange + ActiveModifiers.Sum(m => m.RangeBonus);
    public int MobilityValue => Mathf.Max(0, Mobility + ActiveModifiers.Sum(m => m.MobilityBonus));

    // Calculate full Hp pool of an Unit
    public void InitializeHp()
    {
        CurrentHp = SoldierCount * Durability;
    }

    // Add modifier to Unit
    public void AddModifier(StatModifier modifier)
    {
        if (ActiveModifiers.Any(m => m.Id == modifier.Id))
        {
            Debug.Log($"Unit already has an active modifier: {modifier.DisplayName}");
            return;
        }

        ActiveModifiers.Add(modifier);
    }

    // Remove modifier from Unit
    public void RemoveModifier(string modifierId)
    {
        ActiveModifiers.RemoveAll(m => m.Id == modifierId);
    }

    // Remove all modifiers from Unit
    public void ClearAllModifiers()
    {
        ActiveModifiers.Clear();
    }

    /// <summary>
    /// Obsługuje ręczne modyfikacje wartości statystyk (+/-) z małych pól panelu bocznego
    /// </summary>
    /// <param name="statName"></param>
    /// <param name="deltaValue"></param>
    public void ApplyIncrementalModifier(string statName, int deltaValue)
    {
        if (deltaValue == 0) return;

        string modId = $"ManualOverride_{statName}";
        var existingMod = ActiveModifiers.FirstOrDefault(m => m.Id == modId);

        if (existingMod == null)
        {
            existingMod = new StatModifier { Id = modId, DisplayName = $"Ręczna Edycja: {statName}" };
            AddModifier(existingMod);
        }

        switch (statName)
        {
            case "Attack": existingMod.AttackBonus += deltaValue; break;
            case "Defence": existingMod.DefenceBonus += deltaValue; break;
            case "RangedAttack": existingMod.RangedAttackBonus += deltaValue; break;
            case "RangedDefence": existingMod.RangedDefenceBonus += deltaValue; break;
            case "Range": existingMod.RangeBonus += deltaValue; break;
            case "Mobility": existingMod.MobilityBonus += deltaValue; break;
            case "Morale": existingMod.MoraleBonus += deltaValue; break;
        }
    }

    //public void ApplyDeploymentSoldierDelta(int deltaValue)
    //{
    //    if (deltaValue == 0) return;

    //    SoldierCount = Mathf.Clamp(SoldierCount + deltaValue, 0, TemplateSoldierCount);
    //    StartingSoldierCount = SoldierCount;

    //    InitializeHp();
    //    IsBroken = false;
    //}

    public void SetDeploymentSoldierCount(int exactValue)
    {
        SoldierCount = Mathf.Clamp(exactValue, 0, TemplateSoldierCount);
        StartingSoldierCount = SoldierCount;

        InitializeHp();
        IsBroken = false;
    }

    public Unit CloneUnit()
    {
        var clone = new Unit
        {
            Id = Guid.NewGuid().ToString(),
            Faction = this.Faction,
            UnitName = this.UnitName,
            BaseAttack = this.BaseAttack,
            BaseDefence = this.BaseDefence,
            BaseMorale = this.BaseMorale,
            BaseRangedAttack = this.BaseRangedAttack,
            BaseRangedDefence = this.BaseRangedDefence,
            BaseRange = this.BaseRange,
            Mobility = this.Mobility,
            Durability = this.Durability,
            TemplateSoldierCount = this.TemplateSoldierCount,
            SoldierCount = this.SoldierCount,
            StartingSoldierCount = this.StartingSoldierCount,
            VisualData = this.VisualData
        };

        clone.InitializeHp();

        return clone;
    }
}
