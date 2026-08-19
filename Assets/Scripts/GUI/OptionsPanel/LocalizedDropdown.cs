using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(TMP_Dropdown))]
public class LocalizedDropdown : MonoBehaviour
{
    [Header("Tłumaczenia opcji)")]
    public LocalizedString[] localizedOptions;

    private TMP_Dropdown dropdown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;

        UpdateDropdownOptions();
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(Locale locale)
    {
        UpdateDropdownOptions();
    }

    private void UpdateDropdownOptions()
    {
        if (dropdown == null || localizedOptions.Length == 0) return;

        int currentValue = dropdown.value;

        dropdown.ClearOptions();
        List<string> newOptions = new List<string>();

        foreach (var localizedString in localizedOptions)
        {
            newOptions.Add(localizedString.GetLocalizedString());
        }

        dropdown.AddOptions(newOptions);

        dropdown.value = currentValue;
        dropdown.RefreshShownValue();
    }
}
