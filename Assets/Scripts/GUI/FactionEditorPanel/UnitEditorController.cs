using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitEditorController : MonoBehaviour
{
    [Header("Panele stats/badge")]
    public GameObject statsSection;
    public GameObject badgeEditorSection;

    [Header("Formularz Statystyk")]
    public TMP_InputField inputUnitName;
    public TMP_InputField inputBaseAttack;
    public TMP_InputField inputBaseDefence;
    public TMP_InputField inputRangedAttack;
    public TMP_InputField inputRangedDefence;
    public TMP_InputField inputRange;
    public TMP_InputField inputMobility;
    public TMP_InputField inputDurability;
    public TMP_InputField inputMorale;
    public TMP_InputField inputSoldierCount;

    [Header("Przyciski (Nawigacja)")]
    public Button btnEditBadge;
    public Button btnConfirmBadge;
    public Button btnSaveUnit;
    public Button btnDiscardUnit;
    public Button btnCreateUnit;
    public Button btnDeleteUnits;

    [Header("Stan Edytora (Logika)")]
    private bool isEditingMode = false;
    private Unit currentEditedUnit = null;

    [Header("Referencje Zewnętrzne")]
    public BadgeVisualController badgeVisualController;
    public FactionListController factionListController;

    void Awake()
    {
        TemplateManager.Instance.LoadTemplates();
    }

    void Start()
    {
        ShowStatsSection(); // show statsSection as default

        if (factionListController != null)
        {
            factionListController.OnUnitSelected += LoadUnitToEditor;
        }

        // Navigation Buttons
        btnEditBadge.onClick.AddListener(ShowBadgeEditor);
        btnConfirmBadge.onClick.AddListener(ShowStatsSection);
        btnSaveUnit.onClick.AddListener(SaveUnitFromEditor);
        btnDiscardUnit.onClick.AddListener(() => { ClearEditorForm(); }); // tymczasowo czyścimy formularz

        btnCreateUnit.onClick.AddListener(() =>
        {
            isEditingMode = false;
            currentEditedUnit = null;
            ClearEditorForm();
            ShowStatsSection();
        });

        btnDeleteUnits.onClick.AddListener(DeleteCurrentUnit);
    }

    // button methods

    private void ClearEditorForm()
    {
        inputUnitName.text = "";
        inputBaseAttack.text = "0";
        inputBaseDefence.text = "0";
        inputRangedAttack.text = "0";
        inputRangedDefence.text = "0";
        inputRange.text = "0";
        inputMorale.text = "0";
        inputDurability.text = "0";
        inputMobility.text = "0";
        inputSoldierCount.text = "0";

        bool factionColorLoaded = false;
        string selectedFaction = factionListController.GetSelectedFactionName();

        if (!string.IsNullOrEmpty(selectedFaction))
        {

            if (TemplateManager.Instance.FactionTemplates.TryGetValue(selectedFaction, out FactionData factionData))
            {
                badgeVisualController.ClearVisuals(false);
                badgeVisualController.LoadColor(factionData.ColorHexCode);
                factionColorLoaded = true;
            }
        }

        if (!factionColorLoaded)
        {
            badgeVisualController.ClearVisuals(true);
        }
    }

    private void DeleteCurrentUnit()
    {
        if (isEditingMode && currentEditedUnit != null)
        {
            string idToRemove = currentEditedUnit.Id;

            TemplateManager.Instance.UnitTemplates.Remove(idToRemove);
            TemplateManager.Instance.VisualTemplates.Remove(idToRemove);
            TemplateManager.Instance.SaveTemplates();

            Debug.Log($"Usunięto chorągiew: {currentEditedUnit.UnitName}");

            factionListController.RefreshUnitList();
            ClearEditorForm();
            isEditingMode = false;
            currentEditedUnit = null;
        }
    }

    public void ShowBadgeEditor()
    {
        statsSection.SetActive(false);
        badgeEditorSection.SetActive(true);
    }

    public void ShowStatsSection()
    {
        badgeEditorSection.SetActive(false);
        statsSection.SetActive(true);
    }

    public void LoadUnitToEditor(Unit unitToEdit)
    {
        isEditingMode = true;
        currentEditedUnit = unitToEdit;
        inputUnitName.text = unitToEdit.UnitName;

        // Combat stats
        inputBaseAttack.text = unitToEdit.BaseAttack.ToString();
        inputBaseDefence.text = unitToEdit.BaseDefence.ToString();
        inputRangedAttack.text = unitToEdit.RangedAttack.ToString();
        inputRangedDefence.text = unitToEdit.RangedDefence.ToString();
        inputRange.text = unitToEdit.BaseRange.ToString();

        // Parameters
        inputMorale.text = unitToEdit.Morale.ToString();
        inputDurability.text = unitToEdit.Durability.ToString();
        inputMobility.text = unitToEdit.Mobility.ToString();
        inputSoldierCount.text = unitToEdit.SoldierCount.ToString();

        // Visualisation
        if (TemplateManager.Instance.VisualTemplates.TryGetValue(unitToEdit.Id, out UnitVisualData visualData))
        {
            badgeVisualController.LoadVisualData(visualData);
        }
        else
        {
            badgeVisualController.LoadVisualData(null);
        }

        // Color
        if (!string.IsNullOrEmpty(unitToEdit.Faction) && TemplateManager.Instance.FactionTemplates.TryGetValue(unitToEdit.Faction, out FactionData factionData))
        {
            badgeVisualController.LoadColor(factionData.ColorHexCode);
        }
        else
        {
            badgeVisualController.ClearVisuals(true);
        }

        ShowStatsSection();
    }

    public void SaveUnitFromEditor()
    {
        string selectedFaction = factionListController.GetSelectedFactionName();

        if (!isEditingMode && string.IsNullOrEmpty(selectedFaction))
        {
            Debug.LogWarning("Zapisywanie przerwane: Musisz wybrać lub stworzyć frakcję przed utworzeniem nowej chorągwi!");
            return;
        }

        if (!isEditingMode || currentEditedUnit == null)
        {
            currentEditedUnit = new Unit
            {
                // Assign unique ID to new Unit
                Id = System.Guid.NewGuid().ToString(),
                Faction = selectedFaction
            };
        }

        currentEditedUnit.UnitName = inputUnitName.text;

        // combat stats parse
        currentEditedUnit.BaseAttack = int.TryParse(inputBaseAttack.text, out int att) ? att : 0;
        currentEditedUnit.BaseDefence = int.TryParse(inputBaseDefence.text, out int def) ? def : 0;
        currentEditedUnit.RangedAttack = int.TryParse(inputRangedAttack.text, out int rAtt) ? rAtt : 0;
        currentEditedUnit.RangedDefence = int.TryParse(inputRangedDefence.text, out int rDef) ? rDef : 0;
        currentEditedUnit.BaseRange = int.TryParse(inputRange.text, out int rng) ? rng : 0;

        // parameters parse
        currentEditedUnit.BaseMorale = int.TryParse(inputMorale.text, out int mor) ? mor : 0;
        currentEditedUnit.Durability = int.TryParse(inputDurability.text, out int dur) ? dur : 0;
        currentEditedUnit.Mobility = int.TryParse(inputMobility.text, out int mob) ? mob : 0;
        currentEditedUnit.SoldierCount = int.TryParse(inputSoldierCount.text, out int count) ? count : 0;
        currentEditedUnit.StartingSoldierCount = currentEditedUnit.SoldierCount;

        // Update UnitVisualData object
        UnitVisualData newVisualData = badgeVisualController.GetVisualData(currentEditedUnit.Id);
        // Add/overrite Dictionary in TemplateManager
        TemplateManager.Instance.VisualTemplates[currentEditedUnit.Id] = newVisualData;

        // Update faction color (if Unit has defined faction)
        if (!string.IsNullOrEmpty(currentEditedUnit.Faction) && currentEditedUnit.Faction != "Brak Frakcji")
        {
            FactionData updatedFaction = new FactionData
            {
                Id = currentEditedUnit.Faction,
                ColorHexCode = badgeVisualController.GetCurrentHexCode()
            };
            TemplateManager.Instance.FactionTemplates[currentEditedUnit.Faction] = updatedFaction;
        }

        // Add/overrite unit in database
        TemplateManager.Instance.UnitTemplates[currentEditedUnit.Id] = currentEditedUnit;
        // Save to file
        TemplateManager.Instance.SaveTemplates();

        if (isEditingMode)
        {
            Debug.Log($"Zakutalizowano istniejącą chorągiew: {inputUnitName.text}");
        }
        else
        {
            Debug.Log($"Utworzono nową chorągiew: {inputUnitName.text}");
        }

        factionListController.RefreshUnitList();
        // Return to stats view after saving
        ShowStatsSection();
    }
}
