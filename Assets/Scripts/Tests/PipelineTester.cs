using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PipelineTester : MonoBehaviour
{
    [Header("Instrukcja:")]
    [Tooltip("F1 - Rozstawia bitwę, F2 - Symuluje straty, F3 - Generuje raport")]
    public bool isTesterActive = true;

    void Update()
    {
        if (!isTesterActive) return;

        // F1: Przygotowanie bitwy
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            SetupTestBattle();
        }

        // F2: Symulacja walki i strat
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            SimulateCombat();
        }

        // F3: Zamknięcie bitwy i raport
        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            BattleManager.Instance.EndBattleAndGenerateReport();
        }
    }

    private void SetupTestBattle()
    {
        Debug.Log("[Tester] Inicjalizacja danych z JSON...");
        TemplateManager.Instance.LoadTemplates();

        // 1. Tworzymy fikcyjną konfigurację
        DataManager.Instance.CurrentBattleConfig = new BattleConfiguration(
            "Test_Architektury_01",
            new List<string> { "Imperium Primarii" },
            new List<string> { "Królestwo Jaromaru" }
        );

        // 2. Spawnowanie jednostek używając Twoich ID z plików JSON
        // Imperium Primarii (Atakujący)
        SpawnUnit("3a20c8af-3731-434d-8fad-0737b100053a", new Vector3(-3, 1, 0)); // Principes
        SpawnUnit("e53a6205-5560-4af5-8244-38a44db345cd", new Vector3(-3, -1, 0)); // Hastatii

        // Królestwo Jaromaru (Obrońcy)
        SpawnUnit("0a7e9742-a51e-402d-b6d4-228e37a095da", new Vector3(3, 1, 0)); // Włócznicy
        SpawnUnit("13b6673d-acba-4c3f-8655-bffdf2d6c5dd", new Vector3(3, -1, 0)); // Łucznicy

        // 3. Wymuszamy przejście w stan Combat, żeby odpalić "Turę 0"
        BattleManager.Instance.ChangePhase(BattlePhase.Combat);

        Debug.Log("[Tester] Bitwa rozstawiona. Wciśnij F2, aby zasymulować straty.");
    }

    private void SpawnUnit(string unitId, Vector3 position)
    {
        if (TemplateManager.Instance.UnitTemplates.TryGetValue(unitId, out Unit template))
        {
            Color factionColor = Color.white;
            if (TemplateManager.Instance.FactionTemplates.TryGetValue(template.Faction, out FactionData factionData))
            {
                ColorUtility.TryParseHtmlString(factionData.ColorHexCode, out factionColor);
            }

            BattleManager.Instance.SpawnUnitOnBoard(template, factionColor, position);
        }
        else
        {
            Debug.LogError($"[Tester] Nie znaleziono jednostki o ID: {unitId} w pliku JSON!");
        }
    }

    private void SimulateCombat()
    {
        Debug.Log("[Tester] Symulowanie starcia...");

        UnitToken[] tokens = FindObjectsByType<UnitToken>(FindObjectsInactive.Exclude);

        foreach (var token in tokens)
        {
            // Principes dostają lekkie bęcki
            if (token.UnitData.UnitName == "Principes")
            {
                token.UnitData.SoldierCount -= 30;
                BattleManager.Instance.LogCombatEvent("Włócznicy odpierają szarżę Principes. Principes tracą 30 żołnierzy.");
            }

            // Włócznicy zostają zmasakrowani i uciekają
            if (token.UnitData.UnitName == "Włócznicy")
            {
                token.UnitData.SoldierCount -= 60;
                token.UnitData.IsBroken = true;
                BattleManager.Instance.LogCombatEvent("Hastatii z flanki miażdżą formację Włóczników (60 strat). Morale Włóczników pęka!");
            }

            token.UpdateVisual();
        }

        // Informacja do logu i twardy zapis z podbiciem tury
        BattleManager.Instance.LogCombatEvent("Zakończenie pierwszej fazy starć.");

        // Ponieważ omijamy fizyczną walkę (korutynę z BattleManager), wywołujemy ręcznie przejście tury
        var method = typeof(BattleManager).GetMethod("SaveCurrentBattle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (method != null)
        {
            // Odpalamy autozapis, żeby przetestować czy pakuje historię
            method.Invoke(BattleManager.Instance, new object[] { true });
        }

        Debug.Log("[Tester] Tura zasymulowana, logi zapisane. Wciśnij F3, aby wygenerować raport.");
    }
}
