using UnityEngine;

public class CombatEngine
{
    public void ResolveEngagement(Unit attacker, Unit defender, EngagementType engagement, float distance)
    {
        int totalDamageToDefender = 0;
        int totalDamageToAttacker = 0;

        // Melee engagement logic
        if (engagement == EngagementType.Melee)
        {
            totalDamageToDefender = attacker.Attack;
            totalDamageToAttacker = defender.Defence;
        }
        // Ranged engagement logic
        else if (engagement == EngagementType.Fire)
        {
            if (attacker.Range >= distance) // check if attacker is in range
            {
                totalDamageToDefender = Mathf.Max(0, attacker.RangedAttack - defender.RangedDefence);
            }
            
            if (defender.RangedAttack > 0 && defender.Range >= distance) // check if defender can fire back and is in range
            {
                totalDamageToAttacker = Mathf.Max(0, defender.RangedAttack - attacker.RangedDefence);
            }
        }

        // Apply calculated damage
        defender.CurrentHp = Mathf.Max(0, defender.CurrentHp - totalDamageToDefender);
        attacker.CurrentHp = Mathf.Max(0, attacker.CurrentHp - totalDamageToAttacker);

        // Apply losses, remove number of soldiers equivalent to received damage
        defender.SoldierCount = Mathf.CeilToInt((float)defender.CurrentHp / defender.Durability);
        attacker.SoldierCount = Mathf.CeilToInt((float)attacker.CurrentHp / attacker.Durability);

        CheckMorale(defender);
        CheckMorale(attacker);
    }
    /// <summary>
    /// Checks units current morale if it's below breaking threshold set by Morale value
    /// </summary>
    /// <param name="unit"></param>
    private void CheckMorale (Unit unit)
    {
        if (unit.StartingSoldierCount <= 0) return;

        int lostSoldiers = unit.StartingSoldierCount - unit.SoldierCount;
        float percentLost = ((float)lostSoldiers / unit.StartingSoldierCount) * 100f;

        if (percentLost >= unit.Morale)
        {
            unit.IsBroken = true;
        }
    }
}
