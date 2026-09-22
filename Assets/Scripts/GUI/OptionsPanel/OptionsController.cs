using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class OptionsController : MonoBehaviour
{
    [Header("Elementy UI Opcji")]
    public TMP_Dropdown languageDropdown;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown displayModeDropdown;

    private Resolution[] resolutions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(InitializeLanguageDropdown());
        InitializeResolutionDropdown();
        if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen) displayModeDropdown.value = 0;
        else if (Screen.fullScreenMode == FullScreenMode.Windowed) displayModeDropdown.value = 1;
        else if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow) displayModeDropdown.value = 2;
        displayModeDropdown.onValueChanged.AddListener(SetDisplayMode);
    }

    private IEnumerator InitializeLanguageDropdown()
    {
        yield return LocalizationSettings.InitializationOperation;

        var availableLocales = LocalizationSettings.AvailableLocales.Locales;
        var options = new List<string>();
        int selectedIndex = 0;

        languageDropdown.ClearOptions();

        for (int i = 0; i < availableLocales.Count; i++)
        {
            string localeName = availableLocales[i].Identifier.CultureInfo.NativeName;

            localeName = char.ToUpper(localeName[0]) + localeName.Substring(1);
            options.Add(localeName);

            if (availableLocales[i] == LocalizationSettings.SelectedLocale)
            {
                selectedIndex = i;
            }
        }

        languageDropdown.AddOptions(options);
        languageDropdown.value = selectedIndex;
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    private void OnLanguageChanged(int localeIndex)
    {
        StartCoroutine(SetLocale(localeIndex));
    }

    private IEnumerator SetLocale(int localeIndex)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
    }

    private void InitializeResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
    }

    private void SetDisplayMode(int modeIndex)
    {
        switch (modeIndex)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
    }
}
