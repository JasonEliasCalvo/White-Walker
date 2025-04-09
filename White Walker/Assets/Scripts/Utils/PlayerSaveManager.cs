using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerSaveManager : MonoBehaviour
{
    public static PlayerSaveManager Instance { get; private set; }

    public PlayerSaveData CurrentSave { get; private set; }

    private string savePath => Path.Combine(Application.persistentDataPath, "player_save.json");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadGame();
    }

    public void NewGame()
    {
        CurrentSave = new PlayerSaveData();
        SaveGame();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(CurrentSave, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Partida guardada en: " + savePath);
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            CurrentSave = JsonUtility.FromJson<PlayerSaveData>(json);
            Debug.Log("Partida cargada");
        }
        else
        {
            Debug.Log("No hay partida guardada. Creando nueva.");
            NewGame();
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Partida eliminada.");
        }
    }

    public void UnlockAttack(int id, WeaponType category)
    {
        var data = CurrentSave.unlockData;

        if (!data.unlockedAttacks.ContainsKey(category))
            data.unlockedAttacks[category] = new List<int>();

        if (!data.unlockedAttacks[category].Contains(id))
            data.unlockedAttacks[category].Add(id);
    }

    public bool IsAttackUnlocked(int id, WeaponType category)
    {
        return CurrentSave.unlockData.unlockedAttacks
            .TryGetValue(category, out var list) && list.Contains(id);
    }
}
