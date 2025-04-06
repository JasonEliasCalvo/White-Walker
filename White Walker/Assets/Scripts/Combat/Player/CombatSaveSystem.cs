using System.IO;
using UnityEngine;

public static class CombatSaveSystem
{
    private static string path => Path.Combine(Application.persistentDataPath, "combat.json");

    public static void SaveData(CombatSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Combat data saved to: " + path);
    }

    public static CombatSaveData LoadData()
    {
        if (!File.Exists(path))
        {
            Debug.Log("No save file found. Creating new data.");
            return new CombatSaveData();
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<CombatSaveData>(json);
    }
}
