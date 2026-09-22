using System.Collections.Generic;
using UnityEngine;

public class DeploymentUIManager : MonoBehaviour
{
    public static DeploymentUIManager Instance { get; private set; }

    [Header("Kontenery List")]
    public Transform attackerContentContainer;
    public Transform defenderContentContainer;

    [Header("Prefaby")]
    public GameObject rosterButtonPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PopulateLists(BattleConfiguration config)
    {
        if (config == null)
        {
            Debug.Log("[DeploymentUIManager] Otrzymano pustą konfigurację bitwy!");
            return;
        }

        ClearContainer(attackerContentContainer);
        ClearContainer(defenderContentContainer);

        GenerateButtonsForSide(config.AttackingFactionIds, attackerContentContainer);
        GenerateButtonsForSide(config.DefendingFactionIds, defenderContentContainer);
    }

    private void GenerateButtonsForSide(List<string> factionNames, Transform container)
    {
        if (factionNames == null || factionNames.Count == 0 || container == null) return;

        foreach (var factionName in factionNames)
        {
            Color factionColor = Color.white;

            if (TemplateManager.Instance.FactionTemplates.TryGetValue(factionName, out var factionData))
            {
                if (!ColorUtility.TryParseHtmlString(factionData.ColorHexCode, out factionColor))
                {
                    Debug.LogWarning($"[DeploymentUIManager] Nieprawidłowy format HEX ({factionData.ColorHexCode}) dla frakcji: {factionName}.");
                }
            }
            else
            {
                Debug.LogWarning($"[DeploymentUIManager] Nie znaleziono frakcji: {factionName}. Używam domyślnego koloru.");
            }

            foreach(var unitKvp in TemplateManager.Instance.UnitTemplates)
            {
                Unit unitTemplate = unitKvp.Value;

                if (unitTemplate.Faction == factionName)
                {
                    GameObject btnObj = Instantiate(rosterButtonPrefab, container);
                    RosterButton rosterBtn = btnObj.GetComponent<RosterButton>();

                    if (rosterBtn != null )
                    {
                        rosterBtn.Setup(unitTemplate, factionColor);
                    }
                }
            }
        }
    }

    private void ClearContainer(Transform container)
    {
        if (container == null) return;

        foreach(Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
}
