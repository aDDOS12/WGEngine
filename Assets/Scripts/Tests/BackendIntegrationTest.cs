using System.IO;
using UnityEngine;

public class BackendIntegrationTest : MonoBehaviour
{
    private string testUnitsCsvPath;
    private string testModsCsvPath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("<color=cyan>--- START TESTU INTEGRACYJNEGO BACKENDU ---</color>");

        // 1. Ustalenie ścieżek testowych w bezpiecznym folderze
        testUnitsCsvPath = Path.Combine(Application.persistentDataPath, "test_units.csv");
        testModsCsvPath = Path.Combine(Application.persistentDataPath, "test_modifiers.csv");

        CreateDummyCsvFiles();

        TestCsvImportAndJsonSave();
        TestJsonLoad();
        TestDeepCloning();

        Debug.Log("<color=cyan>--- KONIEC TESTÓW ---</color>");
    }

    private void CreateDummyCsvFiles()
    {
        string unitsCsv = "Id,Faction,UnitName,BaseAttack,BaseDefence,BaseMorale,RangedAttack,RangedDefence,Range,Mobility,Durability,SoldierCount,StartingSoldierCount\nU_TEST_01,Królestwo,Rycerze Testowi,100,80,70,0,20,0,5,50,100,100";
        File.WriteAllText(testUnitsCsvPath, unitsCsv);

        string modsCsv = "Id,DisplayName,AttackBonus,DefenceBonus,RangedAttackBonus,RangedDefenceBonus,MobilityBonus,MoraleBonus\nM_TEST_01,Testowy Sztandar,10,10,0,0,0,5";
        File.WriteAllText(testModsCsvPath, modsCsv);

        Debug.Log("KROK 1: Utworzono tymczasowe pliki CSV do testów.");
    }

    private void TestCsvImportAndJsonSave()
    {
        Debug.Log("<color=orange>--- KROK 2: Test importu CSV i zapisu JSON---</color>");
        //ImportDataFromCsv wywoluje pod spodem CsvImporter
        //a nastepnie aktualizuje TemplateManager i zapisuje do JSON
        DataManager.Instance.ImportDataFromCsv(testUnitsCsvPath, testModsCsvPath);

        int unitCount = TemplateManager.Instance.UnitTemplates.Count;
        int modCount = TemplateManager.Instance.ModifierTemplates.Count;

        Debug.Log($"Szablony załadowane do pamięci: Jednostki = {unitCount}, Modyfikatory = {modCount}");
    }

    private void TestJsonLoad()
    {
        Debug.Log("<color=orange>--- KROK 3: Test wczytywanie z JSON ---</color>");

        TemplateManager.Instance.UnitTemplates.Clear();
        TemplateManager.Instance.ModifierTemplates.Clear();

        TemplateManager.Instance.LoadTemplates();

        Debug.Log($"Szablony w pamięci po załadowaniu z pliku JSON: Jednostki = {TemplateManager.Instance.UnitTemplates.Count}, Modyfikatory = {TemplateManager.Instance.ModifierTemplates.Count}");
    }

    private void TestDeepCloning()
    {
        Debug.Log("<color=orange>--- KROK 4: Test głębokiego klonowania ---</color>");

        DataManager.Instance.ActiveUnits.Clear();

        DataManager.Instance.SpawnUnitFromTemplate("U_TEST_01");
        DataManager.Instance.SpawnUnitFromTemplate("U_TEST_01");

        if (DataManager.Instance.ActiveUnits.Count == 2)
        {
            Unit instanceA = DataManager.Instance.ActiveUnits[0];
            Unit instanceB = DataManager.Instance.ActiveUnits[1];

            Debug.Log($"Utworzono 2 jednostki. Mają wygenerowane unikalne GUID:");
            Debug.Log($"Instancja A -> ID: {instanceA.Id}");
            Debug.Log($"Instancja B -> ID: {instanceB.Id}");

            instanceA.CurrentHp -= 1500;
            instanceA.SoldierCount -= 30;

            Debug.Log($"<color=yellow>Wynik testu izolacji pamięci:</color>");
            Debug.Log($"Instancja A (Uszkodzona) -> Żołnierze: {instanceA.SoldierCount}");
            Debug.Log($"Instancja B (Nietknięta) -> Żołnierze: {instanceB.SoldierCount}");

            // Sprawdzamy czy oryginalny szablon w pamięci pozostał nietknięty
            Unit template = TemplateManager.Instance.UnitTemplates["U_TEST_01"];
            Debug.Log($"Oryginalny szablon (Nietknięty) -> Żołnierze: {template.SoldierCount} (Powinno być 100)");
        }
    }
}
