using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class StatModifier
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public int AttackBonus { get; set; }
    public int DefenceBonus { get; set; }
    public int RangedAttackBonus { get; set; }
    public int RangedDefenceBonus { get; set; }
    public float MobilityBonus { get; set; }
    public float MoraleBonus { get; set; }
    public float RangeBonus { get; set; }
}

// JSON wrapper
public class ModifierDatabase
{
    public List<StatModifier> modifiers;
}
