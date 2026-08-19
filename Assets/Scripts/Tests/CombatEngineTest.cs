using UnityEngine;

public class CombatEngineTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("<color=cyan>--- TEST SILNIKA WALKI ---</color>");

        CombatEngine engine = new CombatEngine();

        Unit attacker = new Unit()
        {
            Id = "U001",
            Faction = "Królewska armia",
            UnitName = "Rycerze",
            BaseAttack = 300,
            BaseDefence = 100,
            BaseMorale = 80,
            Durability = 50,
            SoldierCount = 100,
            StartingSoldierCount = 100,
            RangedAttack = 0,
            RangedDefence = 20,
            BaseRange = 0
        };
        attacker.InitializeHp();

        Unit defender = new Unit()
        {
            Id = "U002",
            Faction = "Rebelianci",
            UnitName = "Chłopi",
            BaseAttack = 50,
            BaseDefence = 50,
            BaseMorale = 40,
            Durability = 30,
            SoldierCount = 100,
            StartingSoldierCount = 100,
            RangedAttack = 0,
            RangedDefence = 10,
            BaseRange = 0
        };
        defender.InitializeHp();

        Debug.Log($"Przed starciem: {attacker.UnitName} (HP: {attacker.CurrentHp}, Żołnierzy: {attacker.SoldierCount}) VS {defender.UnitName} (HP: {defender.CurrentHp}, Żołnierzy: {defender.SoldierCount}");

        Debug.Log("<color=orange>--- STARCIE W ZWARCIU ---</color>");

        engine.ResolveEngagement(attacker, defender, EngagementType.Melee, 1);

        Debug.Log("<color=green>--- ODCZYT WYNIKÓW ---</color>");
        Debug.Log($"ATAKUJĄCY ({attacker.UnitName}): Pozostało żołnierzy: {attacker.SoldierCount}, Aktualne HP: {attacker.CurrentHp}, Rozbici (IsBroken): {attacker.IsBroken}");
        Debug.Log($"OBROŃCA ({defender.UnitName}): Pozostało żołnierzy: {defender.SoldierCount}, Aktualne HP: {defender.CurrentHp}, Rozbici (IsBroken): {defender.IsBroken}");

        Debug.Log("<color=cyan>--- TEST WALKI 2 VS 1 ---</color>");

        Unit attacker1 = new Unit()
        {
            Id = "U003",
            Faction = "Królewska armia",
            UnitName = "Rycerze",
            BaseAttack = 300,
            BaseDefence = 100,
            BaseMorale = 80,
            Durability = 50,
            SoldierCount = 100,
            StartingSoldierCount = 100,
            RangedAttack = 0,
            RangedDefence = 20,
            BaseRange = 0
        };
        attacker1.InitializeHp();

        Unit attacker2 = new Unit()
        {
            Id = "U004",
            Faction = "Królewska armia",
            UnitName = "Ciężka Piechota",
            BaseAttack = 180,
            BaseDefence = 120,
            BaseMorale = 80,
            Durability = 30,
            SoldierCount = 120,
            StartingSoldierCount = 120,
            RangedAttack = 0,
            RangedDefence = 40,
            BaseRange = 0
        };
        attacker2.InitializeHp();

        Unit defender1 = new Unit()
        {
            Id = "U005",
            Faction = "Rebelianci",
            UnitName = "Pikinierzy",
            BaseAttack = 80,
            BaseDefence = 200,
            BaseMorale = 60,
            Durability = 20,
            SoldierCount = 160,
            StartingSoldierCount = 160,
            RangedAttack = 0,
            RangedDefence = 30,
            BaseRange = 0
        };
        defender1.InitializeHp();

        Debug.Log($"Przed starciem: {attacker1.UnitName} (HP: {attacker1.CurrentHp}, Żołnierzy: {attacker1.SoldierCount})" +
            $"& {attacker2.UnitName} (HP: {attacker2.CurrentHp}, Żołnierzy: {attacker2.SoldierCount})" +
            $"VS {defender1.UnitName} (HP: {defender1.CurrentHp}, Żołnierzy: {defender1.SoldierCount}");

        Debug.Log("<color=orange>--- STARCIE W ZWARCIU ---</color>");

        engine.ResolveEngagement(attacker1, defender1, EngagementType.Melee, 1);
        Debug.Log("<color=green>--- ODCZYT WYNIKÓW ---</color>");
        Debug.Log("<color=green>--- STARCIE 1 ---</color>");

        Debug.Log($"ATAKUJĄCY ({attacker1.UnitName}): Pozostało żołnierzy: {attacker1.SoldierCount}, Aktualne HP: {attacker1.CurrentHp}, Rozbici (IsBroken): {attacker1.IsBroken}");
        Debug.Log($"OBROŃCA ({defender1.UnitName}): Pozostało żołnierzy: {defender1.SoldierCount}, Aktualne HP: {defender1.CurrentHp}, Rozbici (IsBroken): {defender1.IsBroken}");

        Debug.Log("<color=green>--- STARCIE 2 ---</color>");

        engine.ResolveEngagement(attacker2, defender1, EngagementType.Melee, 1);
        Debug.Log($"ATAKUJĄCY ({attacker2.UnitName}): Pozostało żołnierzy: {attacker2.SoldierCount}, Aktualne HP: {attacker2.CurrentHp}, Rozbici (IsBroken): {attacker2.IsBroken}");
        Debug.Log($"OBROŃCA ({defender1.UnitName}): Pozostało żołnierzy: {defender1.SoldierCount}, Aktualne HP: {defender1.CurrentHp}, Rozbici (IsBroken): {defender1.IsBroken}");
    }
}
