using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurnLog
{
    public int TurnNumber;
    public List<string> CombatEvents = new List<string>();
}

public class BattleSaveData
{
    public int CurrentTurn;
    public string BattleId;
    public List<string> AttackingFactions = new List<string>();
    public List<string> DefendingFactions = new List<string>();
    public List<UnitSaveData> Units = new List<UnitSaveData>();
    public List<TurnLog> HistoryLogs = new List<TurnLog>();
}

[System.Serializable]
public class UnitSaveData
{
    // Identyfikacja
    public string TemplateId;
    public string UnitName;
    public string FactionID;

    // Pozycja na planszy
    public Vector3 Position;
    public Vector2 FacingDirection;

    // Stan statystyk
    public int StartingSoldierCount;
    public int CurrentSoldierCount;
    public bool IsBroken;
    public List<ModifierSaveData> Modifiers = new List<ModifierSaveData>();
}

[System.Serializable]
public class ModifierSaveData
{
    // Public fields: JsonUtility does not serialize properties
    public string Id;
    public string DisplayName;
    public int AttackBonus;
    public int DefenceBonus;
    public int RangedAttackBonus;
    public int RangedDefenceBonus;
    public float MobilityBonus;
    public float MoraleBonus;
    public float RangeBonus;

    public static ModifierSaveData FromModifier(StatModifier m) => new ModifierSaveData
    {
        Id = m.Id,
        DisplayName = m.DisplayName,
        AttackBonus = m.AttackBonus,
        DefenceBonus = m.DefenceBonus,
        RangedAttackBonus = m.RangedAttackBonus,
        RangedDefenceBonus = m.RangedDefenceBonus,
        MobilityBonus = m.MobilityBonus,
        MoraleBonus = m.MoraleBonus,
        RangeBonus = m.RangeBonus
    };

    public StatModifier ToModifier() => new StatModifier
    {
        Id = Id,
        DisplayName = DisplayName,
        AttackBonus = AttackBonus,
        DefenceBonus = DefenceBonus,
        RangedAttackBonus = RangedAttackBonus,
        RangedDefenceBonus = RangedDefenceBonus,
        MobilityBonus = MobilityBonus,
        MoraleBonus = MoraleBonus,
        RangeBonus = RangeBonus
    };
}
