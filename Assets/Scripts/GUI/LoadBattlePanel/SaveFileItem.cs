using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileItem : MonoBehaviour
{
    [Header("Referencje UI")]
    public TMP_Text textBattleName;
    public TMP_Text textSaveDate;
    public Button loadButton;

    public void Setup(string fileName, DateTime saveDate, Action<string> onLoadClicked)
    {
        // Wyświetlamy nazwę bez rozszerzenia .json, żeby było estetyczniej
        if (textBattleName != null)
        {
            textBattleName.text = fileName.Replace(".json", "");
        }

        // Formatujemy datę (np. 2026-09-10 22:42)
        if (textSaveDate != null)
        {
            textSaveDate.text = saveDate.ToString("yyyy-MM-dd HH:mm");
        }

        // Czyścimy ewentualne stare eventy i podpinamy akcję wczytywania
        if (loadButton != null)
        {
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(() => onLoadClicked(fileName));
        }
    }
}
