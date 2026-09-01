using UnityEngine;

public class SoldierLogicTester : MonoBehaviour
{
    void Start()
    {
        RunTests();
    }

    private void RunTests()
    {
        Debug.Log("--- START TESTÓW LOGIKI ŻOŁNIERZY I MORALE ---");

        // 1. Inicjalizacja jednostki (Stan 120/120, Ucieka przy 50% strat, 10 HP na model)
        Unit testUnit = new Unit
        {
            UnitName = "Testowi Włócznicy",
            TemplateSoldierCount = 120,
            StartingSoldierCount = 120,
            SoldierCount = 120,
            Durability = 10,
            BaseMorale = 50
        };
        testUnit.InitializeHp();

        Debug.Log($"1. Stan surowy: Szablon: {testUnit.TemplateSoldierCount} | Start: {testUnit.StartingSoldierCount} | Aktualnie: {testUnit.SoldierCount}");

        // 2. Symulacja strat z mapy kampanii
        testUnit.SetDeploymentSoldierCount(102);

        Debug.Log($"2. Po stratach przedbitewnych: Szablon: {testUnit.TemplateSoldierCount} | Start: {testUnit.StartingSoldierCount} | Aktualnie: {testUnit.SoldierCount}");

        if (testUnit.StartingSoldierCount == 102 && testUnit.TemplateSoldierCount == 120)
        {
            Debug.Log("<color=green>SUKCES:</color> Nowy punkt odniesienia dla bitwy to 102, ale pamięć o 120 max zachowana.");
        }
        else
        {
            Debug.LogError("<color=red>BŁĄD:</color> Błędne przypisanie zmiennych po wywołaniu delty.");
        }

        // 3. Inicjalizacja walki
        CombatEngine engine = new CombatEngine();

        // Przeciwnik zadający równe 500 obrażeń (zabije dokładnie 50 modeli przy Durability 10)
        Unit attacker = new Unit
        {
            UnitName = "Atakujący",
            BaseAttack = 500,
            Durability = 10,
            SoldierCount = 100,
            StartingSoldierCount = 100
        };
        attacker.InitializeHp();

        // 4. Pierwsze starcie
        engine.ResolveEngagement(attacker, testUnit, EngagementType.Melee, 0);

        Debug.Log($"3. Walka 1 (Strata 50 żołnierzy). Aktualny stan: {testUnit.SoldierCount}/{testUnit.StartingSoldierCount}. Ucieczka: {testUnit.IsBroken}");

        if (!testUnit.IsBroken)
        {
            Debug.Log("<color=green>SUKCES:</color> Jednostka utrzymała pozycję. Zginęło 50 żołnierzy (strata ze 102 = ~49%).");
        }
        else
        {
            Debug.LogError("<color=red>BŁĄD:</color> Jednostka uciekła mimo nieosiągnięcia progu 50% ze 102!");
        }

        // 5. Drugie starcie - dobicie jeszcze jednego modelu (10 obrażeń)
        attacker.BaseAttack = 10;
        engine.ResolveEngagement(attacker, testUnit, EngagementType.Melee, 0);

        Debug.Log($"4. Walka 2 (Strata 1 żołnierza). Aktualny stan: {testUnit.SoldierCount}/{testUnit.StartingSoldierCount}. Ucieczka: {testUnit.IsBroken}");

        if (testUnit.IsBroken)
        {
            Debug.Log("<color=green>SUKCES:</color> Jednostka prawidłowo rozbita. Straty osiągnęły 51/102 (50% nowego stanu bazowego).");
        }
        else
        {
            Debug.LogError("<color=red>BŁĄD:</color> Jednostka nie uciekła, mimo że straciła 50% bazowego stanu!");
        }

        Debug.Log("--- KONIEC TESTÓW ---");
    }
}
