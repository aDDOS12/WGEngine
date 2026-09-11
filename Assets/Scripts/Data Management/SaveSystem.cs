using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SaveSystem
{
    private static readonly string SaveDirectory = Application.persistentDataPath + "/Saves/";

    [System.Serializable]
    private class SaveWrapper
    {
        public string checksum;
        public string dataJson;
    }

    public static void SaveGame(string fileName, BattleSaveData data)
    {
        if (!Directory.Exists(SaveDirectory)) Directory.CreateDirectory(SaveDirectory);

        string path = SaveDirectory + fileName;

        string jsonData = JsonUtility.ToJson(data, true);
        string hash = GenerateHash(jsonData);

        SaveWrapper wrapper = new SaveWrapper { checksum = hash, dataJson = jsonData };
        string finalJson = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(path, finalJson);
        Debug.Log($"[SaveSystem] Zapisano stan bitwy: {path}");
    }

    public static BattleSaveData LoadGame(string fileName)
    {
        string path = SaveDirectory + fileName;
        if (!File.Exists(path))
        {
            Debug.LogError($"[SaveSystem] Plik zapisu nie istnieje: {path}");
            return null;
        }

        string finalJson = File.ReadAllText(path);
        SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(finalJson);

        string currentHash = GenerateHash(wrapper.dataJson);

        // Sprawdzanie integralności pliku
        if (currentHash != wrapper.checksum)
        {
            Debug.LogWarning("[SaveSystem] UWAGA: Wykryto modyfikację pliku zapisu! Suma kontrolna się nie zgadza.");
            // Nie blokujemy ładowania, zostawiamy to do decyzji GM-a, ale ostrzegamy
        }

        return JsonUtility.FromJson<BattleSaveData>(wrapper.dataJson);
    }

    private static string GenerateHash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes) builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }

    public static void DeleteSave(string fileName)
    {
        string path = SaveDirectory + fileName;
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveSystem] Trwale usunięto plik zapisu: {fileName}");
        }
        else
        {
            Debug.LogWarning($"[SaveSystem] Próba usunięcia nieistniejącego pliku: {path}");
        }
    }
}
