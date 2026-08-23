using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public UnitToken HighlightedUnit { get; private set; }

    [Header("Ustawienia Spawnera")]
    public GameObject unitTokenPrefab;
    public Transform tokenContainer;

    private List<UnitToken> activeUnitsOnBoard = new List<UnitToken>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (TemplateManager.Instance.UnitTemplates.Count == 0)
        {
            TemplateManager.Instance.LoadTemplates();
        }

        DeployTestArmies();
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

    private void DeployTestArmies()
    {
        if (unitTokenPrefab == null)
        {
            Debug.Log("[BattleManager] Brakuje referencji do prefabu pionka!");
            return;
        }

        DataManager.Instance.ActiveUnits.Clear();

        var templates = TemplateManager.Instance.UnitTemplates.Values.ToList();

        Vector3 spawnPositon = new Vector3(-5f, 0f, 0f);
        float spacing = 1.5f;

        foreach (var template in templates)
        {
            DataManager.Instance.SpawnUnitFromTemplate(template.Id);

            Unit combatUnit = DataManager.Instance.ActiveUnits.Last();

            Color factionColor = Color.white;
            if (TemplateManager.Instance.FactionTemplates.TryGetValue(combatUnit.Faction, out FactionData factionData))
            {
                if (ColorUtility.TryParseHtmlString(factionData.ColorHexCode, out Color parsedColor))
                {
                    factionColor = parsedColor;
                }
            }

            GameObject tokenObj = Instantiate(unitTokenPrefab, spawnPositon, Quaternion.identity, tokenContainer);
            tokenObj.name = $"Token_{combatUnit.UnitName}";

            UnitToken tokenScript = tokenObj.GetComponent<UnitToken>();
            if (tokenScript != null)
            {
                tokenScript.InitializeUnit(combatUnit, combatUnit.VisualData, factionColor);
                activeUnitsOnBoard.Add(tokenScript);
            }

            spawnPositon.x += spacing;
        }

        Debug.Log($"[BattleManager] Pomyślnie zespawnowano {activeUnitsOnBoard.Count} chorągwii na podstawie {TemplateManager.Instance.UnitTemplates.Count} szablonów");
    }
}
