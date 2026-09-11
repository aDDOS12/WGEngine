using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleSetupUI : MonoBehaviour
{
    [Header("Referencje UI")]
    public TMP_InputField battleNameInput;
    public Transform rowsContainer;
    public GameObject factionRowPrefab;
    public GameObject battleStartPanel;
    public GameObject battleInterface;

    [Header("Przyciski")]
    public Button startBattlebBtn;
    public Button addRowBtn;
    public Button returnBtn;

    private List<FactionRow> activeRows = new List<FactionRow>();
    private List<string> cachedFactions = new List<string>();

    void Start()
    {
        if (!string.IsNullOrEmpty(DataManager.Instance.PendingSaveFileToLoad))
        {
            if (battleStartPanel != null) battleStartPanel.SetActive(false);
            if (battleInterface != null) battleInterface.SetActive(true);

            BattleManager.Instance.LoadBattleFromSave(DataManager.Instance.PendingSaveFileToLoad);

            DataManager.Instance.PendingSaveFileToLoad = string.Empty;
            return;
        }

        if (battleStartPanel != null) battleStartPanel.SetActive(true);
        if (battleInterface != null) battleInterface.SetActive(false);

        if (TemplateManager.Instance.FactionTemplates.Count == 0)
        {
            TemplateManager.Instance.LoadTemplates(); // Sprawdzenie czy dane sa zaladowane
        }
        cachedFactions = new List<string>(TemplateManager.Instance.FactionTemplates.Keys);

        addRowBtn.onClick.AddListener(AddNewRow);
        startBattlebBtn.onClick.AddListener(StartBattle);
        returnBtn.onClick.AddListener(ReturnToMenu);

        AddNewRow();
    }

    private void AddNewRow()
    {
        GameObject rowObj = Instantiate(factionRowPrefab, rowsContainer);
        FactionRow rowScript = rowObj.GetComponent<FactionRow>();

        if (rowScript != null )
        {
            rowScript.Initialize(cachedFactions);
            activeRows.Add(rowScript);
        }
    }

    private void StartBattle()
    {
        if (string.IsNullOrEmpty(battleNameInput.text))
        {
            Debug.LogWarning("[BattleSetupUI] Musisz podać nazwę bitwy, aby kontynuować!");
            // TODO: referencja do bledu tekstu
            return;
        }

        string battleId = battleNameInput.text;

        List<string> defenders = new List<string>();
        List<string> attackers = new List<string>();

        foreach(var row in activeRows)
        {
            string defender = row.GetDefender();
            string attacker = row.GetAttacker();

            if (defender != "Brak" && !defenders.Contains(defender)) defenders.Add(defender);
            if (attacker != "Brak" && !attackers.Contains(attacker)) attackers.Add(attacker);
        }

        if (defenders.Count == 0 && attackers.Count == 0)
        {
            Debug.LogWarning("[BattleSetupUI] Musisz przypisać co najmniej jedną frakcję, aby rozpocząć bitwę!");
            return;
        }

        // Po udanym zaladowaniu tworzymy konfiguracje i wstrzykujemy ja do systemu pamieci
        DataManager.Instance.CurrentBattleConfig = new BattleConfiguration(battleId, attackers, defenders);
        Debug.Log($"[BattleSetupUI] Skonfigurowano bitwę: {battleId}. Zamykam panel ustawień.");

        ChangePanels();
        BattleManager.Instance.ChangePhase(BattlePhase.Deployment);

    }

    private void ChangePanels()
    {
        battleStartPanel.SetActive(false);
        battleInterface.SetActive(true);
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
