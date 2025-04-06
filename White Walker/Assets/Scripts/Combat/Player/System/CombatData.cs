using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ComboData
{
    public Dictionary<AttackCategory, List<int>> equippedCombos = new();
}

[Serializable]
public class PlayerUnlockData
{
    public Dictionary<AttackCategory, List<int>> unlockedAttacks = new();
}

[Serializable]
public class PlayerSaveData
{
    public PlayerUnlockData unlockData = new();
    public ComboData comboData = new();
    public PlayerStatsData statsData = new();
    public SettingsData settingsData = new();
}

[Serializable]
public class PlayerStatsData
{
    public int health = 100;
    public int level = 1;
    public int experience = 0;
    public int money = 0;
}

[Serializable]
public class SettingsData
{
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public string language = "en";
}

public class CombatData : MonoBehaviour
{
    [Header("Combat Database")]
    [SerializeField] private CombatDatabaseSO database;
    [SerializeField] private PlayerSaveData playerData;
    private ComboData comboData = new();

    private Dictionary<AttackCategory, List<int>> equippedCombos = new();
    private Dictionary<AttackCategory, HashSet<int>> unlockedAttacks = new();



    private void Awake()
    {
        InitializeCategories();
    }

    private void InitializeCategories()
    {
        foreach (AttackCategory category in Enum.GetValues(typeof(AttackCategory)))
        {
            if (!equippedCombos.ContainsKey(category))
                equippedCombos[category] = new List<int>();

            if (!unlockedAttacks.ContainsKey(category))
                unlockedAttacks[category] = new HashSet<int>();
        }
    }

    // ==========================
    // COMBOS
    // ==========================

    public void SetCombo(AttackCategory category, List<int> combo)
    {
        equippedCombos[category] = combo;
    }

    public List<int> GetEquippedCombo(AttackCategory category)
    {
        if (equippedCombos.TryGetValue(category, out var list))
            return list;

        return new List<int>();
    }

    // ==========================
    // DESBLOQUEOS
    // ==========================

    public void UnlockAttack(int id, AttackCategory category)
    {
        if (!playerData.unlockData.unlockedAttacks.ContainsKey(category))
            playerData.unlockData.unlockedAttacks[category] = new List<int>();

        if (!playerData.unlockData.unlockedAttacks[category].Contains(id))
            playerData.unlockData.unlockedAttacks[category].Add(id);
    }

    public bool IsUnlocked(int id, AttackCategory category)
    {
        if (playerData.unlockData.unlockedAttacks.TryGetValue(category, out var list))
            return list.Contains(id);
        return false;
    }

    public List<int> GetUnlockedAttackIDs(AttackCategory category)
    {
        if (unlockedAttacks.TryGetValue(category, out var hash))
            return new List<int>(hash);

        return new List<int>();
    }

    // ==========================
    // GUARDADO Y CARGA
    // ==========================

    public PlayerSaveData GetDataForSaving()
    {
        var saveData = new PlayerSaveData();

        foreach (var kvp in equippedCombos)
            saveData.comboData.equippedCombos[kvp.Key] = new List<int>(kvp.Value);

        foreach (var kvp in unlockedAttacks)
            saveData.unlockData.unlockedAttacks[kvp.Key] = new List<int>(kvp.Value);

        return saveData;
    }

    public void LoadFromData(PlayerSaveData data)
    {
        InitializeCategories(); // Asegura que existan todas las categorías

        equippedCombos.Clear();
        unlockedAttacks.Clear();

        foreach (var kvp in data.comboData.equippedCombos)
            equippedCombos[kvp.Key] = new List<int>(kvp.Value);

        foreach (var kvp in data.unlockData.unlockedAttacks)
            unlockedAttacks[kvp.Key] = new HashSet<int>(kvp.Value);
    }

    // ==========================
    // UTILIDAD (opcional)
    // ==========================

    public void SaveUnlockedAttacks()
    {
        // Ejemplo de cómo guardar:
        var data = GetDataForSaving();
        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString("CombatData", json);
    }

    public void LoadUnlockedAttacks()
    {
        if (PlayerPrefs.HasKey("CombatData"))
        {
            string json = PlayerPrefs.GetString("CombatData");
            var data = JsonUtility.FromJson<PlayerSaveData>(json);
            LoadFromData(data);
        }
    }
}
