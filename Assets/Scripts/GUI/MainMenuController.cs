using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject factionEditorPanel;
    public GameObject modifierEditorPanel;
    public GameObject optionsPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);

        if (factionEditorPanel != null) factionEditorPanel.SetActive(false);
        if (modifierEditorPanel != null) modifierEditorPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }
    // Method for "New Battle" button
    public void OnNewBattleClicked()
    {
        //Debug.Log("Ładowanie pustej planszy bitwy...");
        SceneManager.LoadScene("BattleScene");
    }

    // Method for "Faction Editor" button
    public void OnFactionEditorClicker()
    {
        //Debug.Log("Otwieranie Edytora Frakcji...");
        // TODO: logika wczytywania edytora
        mainMenuPanel.SetActive(false);
        factionEditorPanel.SetActive(true);
    }

    // Method for "Modifier Edition" button
    public void OnModifierEditorClicked()
    {
        //Debug.Log("Otwieranie Edytora Modyfikatorów...");
        // TODO: logika wczytywania edytora
        mainMenuPanel.SetActive(false);
        modifierEditorPanel.SetActive(true);
    }

    // Method for "Options" button
    public void OnOptionsClicked()
    {
        //Debug.Log("Otwieranie menu opcji...");
        // TODO: panel UI z opcjami
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    // Method for "Quit" button
    public void OnQuitClicked()
    {
        Debug.Log("Zamykanie aplikacji...");
        Application.Quit();
    }
}
