using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // Method for "New Battle" button
    public void OnNewBattleClicked()
    {
        Debug.Log("Ładowanie pustej planszy bitwy...");
        // SceneManager.LoadScene("BattleScene); //placeholder
    }

    // Method for "Faction Editor" button
    public void OnFactionEditorClicker()
    {
        Debug.Log("Otwieranie Edytora Frakcji...");
        // TODO: logika wczytywania edytora
    }

    // Method for "Modifier Edition" button
    public void OnModifierEditorClicked()
    {
        Debug.Log("Otwieranie Edytora Modyfikatorów...");
        // TODO: logika wczytywania edytora
    }

    // Method for "Options" button
    public void OnOptionsClicked()
    {
        Debug.Log("Otwieranie menu opcji...");
        // TODO: panel UI z opcjami
    }

    // Method for "Quit" button
    public void OnQuitClicked()
    {
        Debug.Log("Zamykanie aplikacji...");
        Application.Quit();
    }
}
