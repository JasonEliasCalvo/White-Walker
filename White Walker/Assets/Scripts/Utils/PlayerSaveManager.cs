using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

[Serializable]
public class PlayerSaveData
{
    public PlayerUnlockData unlockData = new();
    public PlayerOwnedData ownedData = new();
    public PlayerComboData comboData = new();
    public PlayerStatsData playerStats = new();
    public SettingsData settingsData = new();
}

[Serializable]
public class PlayerStatsData
{
    public int health = 100;
    public int level = 1;
    public int experience = 0;
    public int money = 200;
}

[Serializable]
public class PlayerAttackState
{
    public AttackBase attack;
    public bool unlocked;
    public bool owned;
}

[Serializable]
public class PlayerComboData
{
    public List<ComboSet> equippedCombos = new();
}

[Serializable]
public class ComboSet
{
    public WeaponType weaponType;
    public List<int> attackIDs = new();
}
[Serializable]
public class PlayerOwnedData
{
    public Dictionary<WeaponType, List<int>> ownedAttacks = new();
}

public class PlayerUnlockData
{
    public Dictionary<WeaponType, List<int>> unlockedAttacks = new();
}

[Serializable]
public class SettingsData
{
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
}

public enum SaveMode { Json, Scriptable }

public class PlayerSaveManager : MonoBehaviour
{
    public static PlayerSaveManager Instance { get; private set; }

    public SaveMode currentMode = SaveMode.Scriptable;
    public PlayerSaveData CurrentSave { get; private set; }

    public PlayerInventory inventorySO;

    private string savePath => Path.Combine(Application.persistentDataPath, "player_save.json");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            if (transform.root == transform)
                DontDestroyOnLoad(gameObject);
            else
                DontDestroyOnLoad(transform.root.gameObject);
        }

        if (currentMode == SaveMode.Json)
        {
            Debug.Log("Partida guardada en: " + savePath);
            LoadGame();
        }
        else
            CreateFromScriptable();
    }

    void CreateFromScriptable()
    {
        var comboData = new PlayerComboData();

        foreach (var atk in inventorySO.ownedAttacks)
        {
            if (!comboData.equippedCombos.Any(c => c.weaponType == atk.category))
                comboData.equippedCombos.Add(new ComboSet
                {
                    weaponType = atk.category,
                    attackIDs = new List<int>()
                });
        }

        var unlockData = new PlayerUnlockData();
        var ownedData = new PlayerOwnedData();

        foreach (var atk in inventorySO.unlockedAttacks)
        {
            if (!unlockData.unlockedAttacks.ContainsKey(atk.category))
                unlockData.unlockedAttacks[atk.category] = new List<int>();
            unlockData.unlockedAttacks[atk.category].Add(atk.attackID);
        }

        foreach (var atk in inventorySO.ownedAttacks)
        {
            if (!ownedData.ownedAttacks.ContainsKey(atk.category))
                ownedData.ownedAttacks[atk.category] = new List<int>();
            ownedData.ownedAttacks[atk.category].Add(atk.attackID);
        }

        CurrentSave = new PlayerSaveData
        {
            unlockData = unlockData,
            ownedData = ownedData,
            comboData = comboData,
            playerStats = new PlayerStatsData { money = inventorySO.gold }
        };

        SaveGame();
        Debug.Log("Datos convertidos desde ScriptableObject.");
    }

    public void NewGame()
    {
        CurrentSave = new PlayerSaveData();
        SaveGame();
    }

    public void SaveGame()
    {
        if(currentMode != SaveMode.Json)
        {
            Debug.LogWarning("Modo Scriptable no guarda en disco.");
            return;
        }

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

    public void EquipCombo(WeaponType weaponType, List<int> attackIDs)
    {
        if (CurrentSave == null || CurrentSave.comboData == null)
        {
            Debug.LogWarning("No hay datos de guardado activos.");
            return;
        }

        var combos = CurrentSave.comboData.equippedCombos;

        // Eliminar el combo anterior si ya existía uno para este tipo de arma
        combos.RemoveAll(c => c.weaponType == weaponType);

        // Agregar el nuevo combo
        combos.Add(new ComboSet
        {
            weaponType = weaponType,
            attackIDs = new List<int>(attackIDs)
        });

        SaveGame();
        Debug.Log($"Combo equipado para {weaponType} con ataques: {string.Join(", ", attackIDs)}");
        // Ejemplo de uso
        // PlayerSaveManager.Instance.EquipCombo(WeaponType.Claw, new List<int> { 1, 2, 3 });
    }

    public void UnlockAttack(int id, WeaponType category)
    {
        var data = CurrentSave.unlockData;

        if (!data.unlockedAttacks.ContainsKey(category))
            data.unlockedAttacks[category] = new List<int>();

        if (!data.unlockedAttacks[category].Contains(id))
            data.unlockedAttacks[category].Add(id);
    }

    public void PurchaseAttack(int id, WeaponType category)
    {
        var data = CurrentSave.ownedData;

        if (!data.ownedAttacks.ContainsKey(category))
            data.ownedAttacks[category] = new List<int>();

        if (!data.ownedAttacks[category].Contains(id))
            data.ownedAttacks[category].Add(id);
    }

    public bool IsAttackUnlocked(int id, WeaponType category)
    {
        return CurrentSave.unlockData.unlockedAttacks.TryGetValue(category, out var list)
               && list.Contains(id);
    }

    public bool IsAttackOwned(int id, WeaponType category)
    {
        return CurrentSave.ownedData.ownedAttacks.TryGetValue(category, out var list)
               && list.Contains(id);
    }
}
