using System;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ModifierPanelUI : MonoBehaviour
{
    [Header("Konfiguracja Panelu")]
    public bool isAttackerPanel;
    [Tooltip("Obiekt grupujący wszystkie teksty, pozwalający ukryć zawartość, gdy brak wybranej jednostki")]
    public GameObject contentContainer;

    [Header("Referencje Statystyk")]
    public TMP_InputField inputAttack;
    public TMP_InputField inputDefence;
    public TMP_InputField inputRangedAttack;
    public TMP_InputField inputRangedDefence;
    public TMP_InputField inputRange;
    public TMP_InputField inputMorale;
    public TMP_InputField inputDurability;
    public TMP_InputField inputMobility;
    public TMP_InputField inputStartingSoldiers;
    public TMP_InputField inputCurrentSoldiers;

    [Header("Pola Zmian (+/-)")]
    public TMP_InputField inputAttackDelta;
    public TMP_InputField inputDefenceDelta;
    public TMP_InputField inputRangedAttackDelta;
    public TMP_InputField inputRangedDefenceDelta;
    public TMP_InputField inputRangeDelta;
    public TMP_InputField inputMoraleDelta;
    public TMP_InputField inputMobilityDelta;

    [Header("Lista Modyfikatorów")]
    public Transform modifierListContent;
    public GameObject modifierListItemPrefab;
    public Color activeModifierColor = new Color(0.6f, 1f, 0.6f, 1f);
    public Color inactiveModifierColor = Color.white;

    private Unit currentUnit;

    private void OnEnable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnUnitSelected += HandleUnitSelected;
            BattleManager.Instance.OnUnitDeselected += HandleUnitDeselected;
        }
        ClearPanel();
    }

    private void OnDisable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnUnitSelected -= HandleUnitSelected;
            BattleManager.Instance.OnUnitDeselected -= HandleUnitDeselected;
        }
    }

    private void Start()
    {
        if (inputCurrentSoldiers != null)
        {
            inputCurrentSoldiers.onEndEdit.AddListener(OnSoldierCountEdited);
        }

        BindDeltaEvent(inputAttackDelta, "Attack");
        BindDeltaEvent(inputDefenceDelta, "Defence");
        BindDeltaEvent(inputRangedAttackDelta, "RangedAttack");
        BindDeltaEvent(inputRangedDefenceDelta, "RangedDefence");
        BindDeltaEvent(inputRangeDelta, "Range");
        BindDeltaEvent(inputMoraleDelta, "Morale");
        BindDeltaEvent(inputMobilityDelta, "Mobility");
    }

    private void BindDeltaEvent(TMP_InputField inputField, string statName)
    {
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener((val) => OnDeltaEdited(val, statName, inputField));
        }
    }

    private void OnDeltaEdited(string inputValue, string statName, TMP_InputField inputField)
    {
        if (currentUnit == null) return;

        if (int.TryParse(inputValue, out int deltaValue) && deltaValue != 0)
        {
            currentUnit.ApplyIncrementalModifier(statName, deltaValue);
            UpdateStatsDisplay(currentUnit);

            if (BattleManager.Instance.HighlightedUnit != null)
            {
                BattleManager.Instance.HighlightedUnit.UpdateVisual();
            }
        }

        inputField.text = "";
    }

    private void OnSoldierCountEdited(string inputValue)
    {
        if (currentUnit == null || BattleManager.Instance.CurrentPhase != BattlePhase.Deployment)
        {
            if (currentUnit != null) UpdateStatsDisplay(currentUnit);
            return;
        }

        if (int.TryParse(inputValue, out int exactValue))
        {
            currentUnit.SetDeploymentSoldierCount(exactValue);
            UpdateStatsDisplay(currentUnit);

            if (BattleManager.Instance.HighlightedUnit != null)
            {
                BattleManager.Instance.HighlightedUnit.UpdateVisual();
            }
            //Debug.Log($"[ModifierPanel] GM ustawił stan żołnierzy: {currentUnit.SoldierCount} dla {currentUnit.UnitName}");
        }
    }

    private void HandleUnitSelected(UnitToken token)
    {
        currentUnit = token.UnitData;
        var config = DataManager.Instance.CurrentBattleConfig;

        bool belongsToAttacker = config.AttackingFactionIds.Contains(currentUnit.Faction);
        bool belongsToDefender = config.DefendingFactionIds.Contains(currentUnit.Faction);

        if ((isAttackerPanel && !belongsToAttacker) || (!isAttackerPanel && !belongsToDefender))
        {
            ClearPanel();
            return;
        }

        contentContainer.SetActive(true);
        UpdateStatsDisplay(currentUnit);
        UpdateModifierList();
    }

    private void HandleUnitDeselected()
    {
        currentUnit = null;
        ClearPanel();
    }

    private void ClearPanel()
    {
        if (contentContainer != null)
        {
            contentContainer.SetActive(false);
        }
    }

    public void UpdateStatsDisplay(Unit unit)
    {
        if (inputAttack != null) inputAttack.SetTextWithoutNotify(unit.Attack.ToString());
        if (inputDefence != null) inputDefence.SetTextWithoutNotify(unit.Defence.ToString());
        if (inputRangedAttack != null) inputRangedAttack.SetTextWithoutNotify(unit.RangedAttack.ToString());
        if (inputRangedDefence != null) inputRangedDefence.SetTextWithoutNotify(unit.RangedDefence.ToString());
        if (inputRange != null) inputRange.SetTextWithoutNotify(unit.Range.ToString());
        if (inputMorale != null) inputMorale.SetTextWithoutNotify(unit.Morale.ToString());
        if (inputDurability != null) inputDurability.SetTextWithoutNotify(unit.Durability.ToString());
        if (inputMobility != null) inputMobility.SetTextWithoutNotify(unit.Mobility.ToString());
        if (inputStartingSoldiers != null) inputStartingSoldiers.SetTextWithoutNotify(unit.TemplateSoldierCount.ToString());
        if (inputCurrentSoldiers != null) inputCurrentSoldiers.SetTextWithoutNotify(unit.SoldierCount.ToString());
    }

    private void UpdateModifierList()
    {
        if (modifierListContent == null || modifierListItemPrefab == null) return;

        // Czyszczenie poprzedniej listy przy zmianie jednostki
        foreach (Transform child in modifierListContent)
        {
            Destroy(child.gameObject);
        }

        if (currentUnit == null) return;

        // Generowanie nowych przycisków na podstawie bazy z TemplateManager
        foreach (var modifierTemplate in TemplateManager.Instance.ModifierTemplates.Values)
        {
            GameObject newItem = Instantiate(modifierListItemPrefab, modifierListContent);

            TMP_Text buttonText = newItem.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = modifierTemplate.DisplayName;
            }

            Image buttonImage = newItem.GetComponent<Image>();
            Button itemButton = newItem.GetComponent<Button>();

            // Weryfikacja czy jednostka aktualnie posiada przypisany ten modyfikator
            bool isActive = currentUnit.ActiveModifiers.Exists(m => m.Id == modifierTemplate.Id);

            if (buttonImage != null)
            {
                buttonImage.color = isActive ? activeModifierColor : inactiveModifierColor;
            }

            if (itemButton != null)
            {
                // Zamknięcie zmiennej w lambdzie (closure) by przypisało odpowiednie ID do onClick
                StatModifier modToApply = modifierTemplate;
                itemButton.onClick.AddListener(() => ToggleModifier(modToApply));
            }
        }
    }

    private void ToggleModifier(StatModifier modifier)
    {
        if (currentUnit == null) return;

        bool hasModifier = currentUnit.ActiveModifiers.Exists(m => m.Id == modifier.Id);

        if (hasModifier)
        {
            currentUnit.RemoveModifier(modifier.Id);
        }
        else
        {
            currentUnit.AddModifier(modifier);
        }

        // Odświeżamy statystyki po lewej i kolory przycisków modyfikatorów
        UpdateStatsDisplay(currentUnit);
        UpdateModifierList();
    }
}
