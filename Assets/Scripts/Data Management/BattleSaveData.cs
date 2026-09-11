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
    public string UnitName;
    public string FactionID;

    // Pozycja na planszy
    public Vector3 Position;
    public Vector2 FacingDirection;

    // Stan statystyk
    public int StartingSoldierCount;
    public int CurrentSoldierCount;
    public bool IsBroken;
}
