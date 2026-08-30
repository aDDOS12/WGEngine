using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public UnitToken HighlightedUnit { get; private set; }
    public BattlePhase CurrentPhase { get; private set; }

    [Header("Referencje Planszy")]
    public Transform tokenContainer;

    private List<UnitToken> activeUnitsOnBoard = new List<UnitToken>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ChangePhase(BattlePhase.Initialization);
    }

    public void ChangePhase(BattlePhase newPhase)
    {
        CurrentPhase = newPhase;
        Debug.Log($"[BattleManager] Zmiana fazy na {CurrentPhase}");

        switch (CurrentPhase)
        {
            case BattlePhase.Initialization:
                ClearBoard();
                break;
            case BattlePhase.Deployment:
                PrepareDeployment();
                break;
            case BattlePhase.Combat:
                // TODO: Logika walki
                break;
        }
    }

    private void PrepareDeployment()
    {
        var config = DataManager.Instance.CurrentBattleConfig;
        if (config == null)
        {
            Debug.LogError("[BattleManager] Błąd krytyczny: Próba rozpoczęcia Deployment bez konfiguracji bitwy!");
            return;
        }

        // Wypełnienie bocznych paneli jednostkami dostępnymi dla wybranych frakcji
        if (DeploymentUIManager.Instance != null)
        {
            DeploymentUIManager.Instance.PopulateLists(config);
        }
        else
        {
            Debug.LogWarning("[BattleManager] Brak instancji DeploymentUIManager na scenie.");
        }
    }

    private void ClearBoard()
    {
        activeUnitsOnBoard.Clear();
        if (tokenContainer != null)
        {
            foreach (Transform child in tokenContainer) Destroy(child.gameObject);
        }
    }

    public void SelectUnit(UnitToken unit)
    {
        if (HighlightedUnit == unit)
        {
            HighlightedUnit.SetSelected(false);
            HighlightedUnit = null;
            Debug.Log("[BattleManager] Odznaczono jednostkę.");
            return;
        }

        if (HighlightedUnit != null)
        {
            HighlightedUnit.SetSelected(false);
        }

        HighlightedUnit = unit;
        HighlightedUnit.SetSelected(true);

        Debug.Log($"[BattleManager] Zaznaczono jednostkę: {HighlightedUnit.UnitData.UnitName}");
    }

    // Do archiwizacji
    //private void InitializeBattle()
    //{
    //    if (TemplateManager.Instance.UnitTemplates.Count == 0)
    //    {
    //        TemplateManager.Instance.LoadTemplates();
    //    }

    //    var config = DataManager.Instance.CurrentBattleConfig;
    //    if (config == null)
    //    {
    //        Debug.LogWarning("[BattleManager] Brak konfiguracji bitwy. Ładowanie trybu awaryjnego/testowego.");
    //        config = new BattleConfiguration("Test_Battle",
    //            new List<string> { "Imperium Primarii" },
    //            new List<string> { "Królestwo Jaromaru" });
    //    }

    //    Vector3 attackerSpawnPos = new Vector3(-5f, -3f, 0f); // Dół ekranu
    //    Vector3 defenderSpawnPos = new Vector3(-5f, 3f, 0f);  // Góra ekranu
    //    float spacing = 1.5f;

    //    foreach (var template in TemplateManager.Instance.UnitTemplates.Values)
    //    {
    //        bool isAttacker = config.AttackingFactionIds.Contains(template.Faction);
    //        bool isDefender = config.DefendingFactionIds.Contains(template.Faction);

    //        if (!isAttacker && !isDefender) continue;

    //        Vector3 currentSpawnPos = isAttacker ? attackerSpawnPos : defenderSpawnPos;

    //        DataManager.Instance.SpawnUnitFromTemplate(template.Id);
    //        Unit combatUnit = DataManager.Instance.ActiveUnits.Last();

    //        Color factionColor = Color.white;
    //        if (TemplateManager.Instance.FactionTemplates.TryGetValue(combatUnit.Faction, out FactionData factionData))
    //        {
    //            if (ColorUtility.TryParseHtmlString(factionData.ColorHexCode, out Color parsedColor))
    //            {
    //                factionColor = parsedColor;
    //            }
    //        }

    //        GameObject tokenObj = Instantiate(unitTokenPrefab, currentSpawnPos, Quaternion.identity, tokenContainer);
    //        tokenObj.name = $"Token_{combatUnit.UnitName}";

    //        UnitToken tokenScript = tokenObj.GetComponent<UnitToken>();
    //        if (tokenScript != null)
    //        {
    //            tokenScript.InitializeUnit(combatUnit, combatUnit.VisualData, factionColor);

    //            if (tokenScript != null)
    //            {
    //                tokenScript.InitializeUnit(combatUnit, combatUnit.VisualData, factionColor);
    //                Vector2 facingDirection = isAttacker ? Vector2.up : Vector2.down;
    //                tokenScript.SetFacingDirection(facingDirection);

    //                activeUnitsOnBoard.Add(tokenScript);
    //            }
    //        }

    //        if (isAttacker)
    //        {
    //            attackerSpawnPos.x += spacing;
    //        }
    //        else
    //        {
    //            defenderSpawnPos.x += spacing;
    //        }
    //    }

    //    int totalFactions = config.AttackingFactionIds.Count + config.DefendingFactionIds.Count;
    //    Debug.Log($"[BattleManager] Zespawnowano {activeUnitsOnBoard.Count} jednostek z {totalFactions} frakcji.");

    //    ChangePhase(BattlePhase.Deployment);
    //}
}
