using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TemplateManager : MonoBehaviour
{
    public List<StatModifier> LoadedTemplates { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadModifiers();
    }

    // Load json file with modifiers
    private void LoadModifiers()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "modifiers.json");

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);

            ModifierDatabase database = JsonUtility.FromJson<ModifierDatabase>(jsonContent);

            LoadedTemplates = database.modifiers;
            Debug.Log($"Succesfuly loade {LoadedTemplates.Count} modifier templates");
        }
        else
        {
            Debug.LogError("modifier config file not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
