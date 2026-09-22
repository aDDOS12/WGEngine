using System.Collections.Generic;

public class BattleConfiguration
{
    public string BattleId { get; private set; }
    public List<string> AttackingFactionIds { get; private set; }
    public List<string> DefendingFactionIds { get; private set; }

    public BattleConfiguration(string battleId, List<string> attackingFactions, List<string> defendingFactionIds)
    {
        BattleId = battleId;
        AttackingFactionIds = attackingFactions ?? new List<string>();
        DefendingFactionIds = defendingFactionIds ?? new List<string>();
    }
}
