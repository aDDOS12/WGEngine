using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [Header("Referencje UI")]
    public TMP_Dropdown dropdownMapSelect;
    public Button btnRefreshList;
    public Button btnLoadMap;

    [Header("Referencje Obiektów")]
    public SpriteRenderer mapRenderer;

    private Texture2D currentTexture;
    private Sprite currentSprite;

    private string mapsDirectoryPath;
    private List<string> availableMapPaths = new List<string>();
    void Start()
    {
        mapsDirectoryPath = Path.Combine(Application.persistentDataPath, "Maps");
        EnsureDirectoryExists();

        btnRefreshList.onClick.AddListener(RefreshMapList);
        btnLoadMap.onClick.AddListener(LoadSelectedMap);

        RefreshMapList();
    }

    private void EnsureDirectoryExists()
    {
        if (!Directory.Exists(mapsDirectoryPath))
        {
            Directory.CreateDirectory(mapsDirectoryPath);
            Debug.Log($"Utworzono folder na mapy: {mapsDirectoryPath}");
        }
    }

    private void RefreshMapList()
    {
        dropdownMapSelect.ClearOptions();
        availableMapPaths.Clear();

        EnsureDirectoryExists();

        string[] files = Directory.GetFiles(mapsDirectoryPath, "*")
            .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jpeg"))
            .ToArray();

        List<string> options = new List<string>();

        if (files.Length == 0)
        {
            options.Add("Brak map w folderze");
        }
        else
        {
            foreach (string file in files)
            {
                availableMapPaths.Add(file);
                options.Add(Path.GetFileName(file));
            }
        }

        dropdownMapSelect.AddOptions(options);
    }

    private void LoadSelectedMap()
    {
        if (availableMapPaths.Count == 0)
        {
            Debug.LogWarning("Brak map do załadowania");
            return;
        }

        int selectedIndex = dropdownMapSelect.value;
        string path = availableMapPaths[selectedIndex];

        try
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);

            if (texture.LoadImage(fileData))
            {
                if (currentTexture != null) Destroy(currentTexture);
                if (currentSprite != null) Destroy(currentSprite);

                currentTexture = texture;
                currentSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);

                mapRenderer.sprite = currentSprite;
                Debug.Log($"Załadowano mapę: {Path.GetFileName(path)}");
            }
            else
            {
                Debug.LogError("Nie udało się zdekodować obrazu.");
                Destroy(texture);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Błąd podczas ładowania mapy: {e.Message}");
        }
    }
}
