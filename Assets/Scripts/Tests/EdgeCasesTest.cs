using UnityEngine;

public class EdgeCasesTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("<color=cyan>--- TESTY PRZYPADKÓW BRZEGOWYCH ---</color>");

        TestRangedCombat();
        TestCustomModifiers();
        TestDataManagerErrorHandling();
    }

    private void TestRangedCombat()
    {
        Debug.Log("<color=orange>--- 1. TEST WALKI DYSTANSOWEJ ---</color>");
        CombatEngine engine = new CombatEngine();

        // Strzelec z dużym zasięgiem (Range = 10)
        Unit archer = new Unit { Id = "archer_01", UnitName = "Łucznicy", CurrentHp = 100, SoldierCount = 50, Durability = 2, RangedAttack = 40, Range = 10, RangedDefence = 10 };
        // Piechur bez ataku dystansowego i z małym zasięgiem (Range = 0)
        Unit infantry = new Unit { Id = "infantry_01", UnitName = "Piechota", CurrentHp = 100, SoldierCount = 50, Durability = 2, RangedAttack = 0, Range = 0, RangedDefence = 15 };

        // Dystans starcia to 8. Łucznik dostrzeli, piechota nie.
        engine.ResolveEngagement(archer, infantry, EngagementType.Fire, 8);

        Debug.Log($"Po ostrzale z dystansu 8:");
        Debug.Log($"Łucznicy (HP: {archer.CurrentHp}) - nie powinni otrzymać obrażeń.");
        Debug.Log($"Piechota (HP: {infantry.CurrentHp}) - powinna otrzymać obrażenia (40 Atak - 15 Obrona = 25 Dmg).");
    }

    private void TestCustomModifiers()
    {
        Debug.Log("<color=orange>--- 2. TEST WŁASNYCH MODYFIKATORÓW (W LOCIE) ---</color>");
        Unit heroUnit = new Unit { UnitName = "Oddział Graczy", BaseAttack = 50, BaseMorale = 50 };

        // Ważne: przed testem odkomentuj this.AddModifier(customMod) w klasie Unit!
        heroUnit.ApplyCustomModifier("Błogosławieństwo MG", 20, 10f);
        heroUnit.ApplyCustomModifier("Szarża z górki", 30, 0f);

        Debug.Log($"Atak po dodaniu dwóch improwizowanych buffów: {heroUnit.Attack} (Oczekiwane 100)");
        Debug.Log($"Ilość aktywnych modyfikatorów: {heroUnit.ActiveModifiers.Count} (Oczekiwane 2)");
    }
    private void TestDataManagerErrorHandling()
    {
        Debug.Log("<color=orange>--- 3. TEST ZABEZPIECZEŃ DATAMANAGERA ---</color>");

        // Próbujemy nałożyć modyfikator na jednostkę zmyślonym ID. 
        // Oczekujemy żółtego ostrzeżenia w konsoli z DataManager.
        DataManager.Instance.ApplyModifierToActiveUnit("ZMYSLONE_ID_JEDNOSTKI", "JAKIS_MODYFIKATOR");

        // Dodajemy fałszywą jednostkę do listy aktywnych
        Unit dummyUnit = new Unit { Id = "real_unit_01", UnitName = "Prawdziwa Jednostka" };
        DataManager.Instance.ActiveUnits.Add(dummyUnit);

        // Próbujemy nałożyć na nią nieistniejący w bazie modyfikator
        // Oczekujemy kolejnego ostrzeżenia, że nie znaleziono szablonu.
        DataManager.Instance.ApplyModifierToActiveUnit("real_unit_01", "ZMYSLONE_ID_MODYFIKATORA");
    }
}
