using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactionListController : MonoBehaviour
{
    [Header("Zarządzania Frakcją i Listą)")]
    public TMP_Dropdown dropdownFactionSelect; // faction select dropdown
    public Transform unitListContentContainer; // "Content" object inside scroll view
    public GameObject unitListItemPrefab; // scroll view button prefab

    [Header("Pop-up Frakcji")]
    public GameObject factionPopupPanel;
    public TMP_InputField inputNewFactionName;
    public Button btnOpenFactionPopup;
    public Button btnConfirmNewFaction;
    public Button btnCancelNewFaction;

    public event Action<Unit> OnUnitSelected;
    void Start()
    {
        factionPopupPanel.SetActive(false);

        btnOpenFactionPopup.onClick.AddListener(() =>
        {
            factionPopupPanel.SetActive(true);
            inputNewFactionName.text = "";
        });

        btnCancelNewFaction.onClick.AddListener(() =>
        {
            factionPopupPanel.SetActive(false);
        });

        btnConfirmNewFaction.onClick.AddListener(CreateNewFaction);

        dropdownFactionSelect.onValueChanged.AddListener(delegate { RefreshUnitList(); });

        RefreshFactionDropdown();
        RefreshUnitList();
    }

    public string GetSelectedFactionName()
    {
        if (dropdownFactionSelect != null && dropdownFactionSelect.options.Count > 0 && dropdownFactionSelect.value > 0)
        {
            return dropdownFactionSelect.options[dropdownFactionSelect.value].text;
        }
        return null;
    }

    public void RefreshFactionDropdown()
    {
        if (dropdownFactionSelect == null) return;

        dropdownFactionSelect.ClearOptions();

        List<string> options = new List<string> { "Wybierz Frakcję" };

        foreach (var faction in TemplateManager.Instance.FactionTemplates.Values)
        {
            options.Add(faction.Id);
        }

        dropdownFactionSelect.AddOptions(options);
    }

    public void RefreshUnitList()
    {
        foreach (Transform child in unitListContentContainer)
        {
            Destroy(child.gameObject);
        }

        string selectedFactionFilter = GetSelectedFactionName();

        if (string.IsNullOrEmpty(selectedFactionFilter))
        {
            return;
        }

        foreach (var unit in TemplateManager.Instance.UnitTemplates.Values)
        {
            if (unit.Faction != selectedFactionFilter)
            {
                continue;
            }

            GameObject newItem = Instantiate(unitListItemPrefab, unitListContentContainer);

            TMP_Text buttonText = newItem.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = string.IsNullOrEmpty(unit.UnitName) ? "Nowa Chorągiew" : unit.UnitName;
            }

            Button itemButton = newItem.GetComponent<Button>();
            if (itemButton != null)
            {
                Unit unitToLoad = unit;
                itemButton.onClick.AddListener(() => { OnUnitSelected?.Invoke(unitToLoad); });
            }
        }
    }

    private void CreateNewFaction()
    {
        string newFactionName = inputNewFactionName.text.Trim();

        if (string.IsNullOrEmpty(newFactionName) || newFactionName == "Wybierz Frakcję")
        {
            Debug.LogWarning("Nazwa frakcji nie może być pusta ani zastrzeżona!");
            return;
        }

        if (TemplateManager.Instance.FactionTemplates.ContainsKey(newFactionName))
        {
            Debug.LogWarning("Frakcja już istnieje!");
            return;
        }

        FactionData newFaction = new FactionData
        {
            Id = newFactionName,
            ColorHexCode = "#FFFFFF"
        };

        TemplateManager.Instance.FactionTemplates.Add(newFactionName, newFaction);
        TemplateManager.Instance.SaveTemplates();
        Debug.Log($"Utworzono nową frakcję: {newFactionName}");

        inputNewFactionName.text = "";
        factionPopupPanel.SetActive(false);
        RefreshFactionDropdown();

        dropdownFactionSelect.value = dropdownFactionSelect.options.Count - 1;
    }
}
