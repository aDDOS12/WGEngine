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
    public int RangedAttack { get; set; }
    public int RangedDefence { get; set; }
    public int BaseRange { get; set; }
    public int Mobility { get; set; }
    public int Durability { get; set; }
    public int SoldierCount { get; set; }
    public int CurrentHp { get; set; }
    public int StartingSoldierCount { get; set; }
    public bool IsBroken { get; set; }
    public UnitVisualData VisualData { get; set; }
    public List<StatModifier> ActiveModifiers { get; private set; } = new List<StatModifier>();
    public int Attack => BaseAttack + ActiveModifiers.Sum(m => m.AttackBonus);
    public int Defence => BaseDefence + ActiveModifiers.Sum(m => m.DefenceBonus);
    public float Morale => BaseMorale + ActiveModifiers.Sum(m => m.MoraleBonus);
    public int Range => BaseRange + ActiveModifiers.Sum(m => m.RangeBonus);

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

    public void ApplyCustomModifier(string customName, int customAttack, float customMorale)
    {
        var customMod = new StatModifier
        {
            Id = $"Custom_{System.Guid.NewGuid()}",
            DisplayName = string.IsNullOrEmpty(customName) ? "Custom Modifier" : customName,
            AttackBonus = customAttack,
            MoraleBonus = customMorale
        };

        this.AddModifier(customMod);
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
            RangedAttack = this.RangedAttack,
            RangedDefence = this.RangedDefence,
            BaseRange = this.BaseRange,
            Mobility = this.Mobility,
            Durability = this.Durability,
            SoldierCount = this.SoldierCount,
            StartingSoldierCount = this.StartingSoldierCount,
            VisualData = this.VisualData
        };

        clone.InitializeHp();

        return clone;
    }
}
