using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoadUI : MonoBehaviour
{
    [Header("Referencje UI")]
    public Transform contentContainer;
    public GameObject saveFileButtonPrefab;
    public Button confirmLoadButton;
    public Button deleteButton;

    private string selectedSaveFile = "";

    private void OnEnable()
    {
        selectedSaveFile = "";
        RefreshSaveList();
    }

    private void Start()
    {
        if (confirmLoadButton != null) confirmLoadButton.onClick.AddListener(LoadSelectedGame);

        // Podpięcie usuwania
        if (deleteButton != null) deleteButton.onClick.AddListener(DeleteSelectedGame);
    }

    public void RefreshSaveList()
    {
        // 1. Czyszczenie starych przycisków
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Szukanie plików .json w folderze
        string savePath = Application.persistentDataPath + "/Saves/";
        if (!Directory.Exists(savePath)) return;

        string[] files = Directory.GetFiles(savePath, "*.json");

        // 3. Generowanie przycisków
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);

            System.DateTime fileDate = File.GetLastWriteTime(file);

            GameObject btnObj = Instantiate(saveFileButtonPrefab, contentContainer);

            // Ustawiamy nazwę pliku na przycisku
            SaveFileItem itemScript = btnObj.GetComponent<SaveFileItem>();
            if (itemScript != null)
            {
                // Przekazujemy metodę zaznaczającą plik
                itemScript.Setup(fileName, fileDate, SelectSaveGame);
            }
        }
    }

    private void SelectSaveGame(string fileName)
    {
        selectedSaveFile = fileName;
        Debug.Log($"[SaveLoadUI] Zaznaczono plik: {selectedSaveFile}");
    }

    private void LoadSelectedGame()
    {
        if (string.IsNullOrEmpty(selectedSaveFile))
        {
            Debug.LogWarning("[SaveLoadUI] Najpierw wybierz zapis z listy!");
            return;
        }

        DataManager.Instance.PendingSaveFileToLoad = selectedSaveFile;
        SceneManager.LoadScene("BattleScene");
    }

    public void DeleteSelectedGame()
    {
        if (string.IsNullOrEmpty(selectedSaveFile))
        {
            Debug.LogWarning("[SaveLoadUI] Najpierw wybierz zapis z listy do usunięcia!");
            return;
        }

        DataManager.Instance.DeleteSaveFile(selectedSaveFile);
        selectedSaveFile = ""; // Czyścimy wybór
        RefreshSaveList(); // Odświeżamy widok po usunięciu
    }
}
