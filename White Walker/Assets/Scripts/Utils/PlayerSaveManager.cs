using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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

    private void AddAttackToDictionary(Dictionary<WeaponType, List<int>> dictionary, WeaponType category, int id)
    {
        // Comprueba si la categoría del arma ya existe como clave en el diccionario.
        if (!dictionary.ContainsKey(category))
        {
            // Si no existe, crea una nueva lista de enteros para esa categoría.
            dictionary[category] = new List<int>();
        }
        // Comprueba si el ID del ataque ya está en la lista de la categoría.
        if (!dictionary[category].Contains(id))
        {
            // Si no está, agrega el ID del ataque a la lista.
            dictionary[category].Add(id);
        }
    }

    void CreateFromScriptable()
    {
        var comboData = new PlayerComboData();

        // Itera a través de todos los ataques poseídos definidos en el Scriptable Object de inventario.
        foreach (var atk in inventorySO.ownedAttacks)
        {
            // Comprueba si ya existe un conjunto de combos equipado para la categoría de arma del ataque.
            if (!comboData.equippedCombos.Any(c => c.weaponType == atk.category))
            {
                // Si no existe, agrega un nuevo ComboSet para esa categoría de arma.
                comboData.equippedCombos.Add(new ComboSet
                {
                    weaponType = atk.category,
                    attackIDs = new List<int>() // Inicializa una nueva lista para los IDs de los ataques.
                });
            }
        }

        // Crea nuevas instancias para almacenar los ataques desbloqueados y poseídos.
        var unlockData = new PlayerUnlockData();
        var ownedData = new PlayerOwnedData();

        // Itera a través de todos los ataques desbloqueados definidos en el Scriptable Object de inventario.
        foreach (var atk in inventorySO.unlockedAttacks)
        {
            // Utiliza la función AddAttackToDictionary para agregar el ID del ataque a la lista de ataques desbloqueados para su categoría.
            AddAttackToDictionary(unlockData.unlockedAttacks, atk.category, atk.attackID);
        }

        // Itera a través de todos los ataques poseídos definidos en el Scriptable Object de inventario.
        foreach (var atk in inventorySO.ownedAttacks)
        {
            // Utiliza la función AddAttackToDictionary para agregar el ID del ataque a la lista de ataques poseídos para su categoría.
            AddAttackToDictionary(ownedData.ownedAttacks, atk.category, atk.attackID);
        }

        // Crea una nueva instancia de PlayerSaveData y asigna los datos creados.
        CurrentSave = new PlayerSaveData
        {
            unlockData = unlockData,
            ownedData = ownedData,
            comboData = comboData,
            playerStats = new PlayerStatsData { points = inventorySO.gold } // Inicializa las estadísticas del jugador con el oro del inventario inicial.
        };

        SaveGame();
        Debug.Log("Datos convertidos desde ScriptableObject.");
    }

    public void NewGame()
    {
        CurrentSave = new PlayerSaveData(); // Crea una nueva instancia de PlayerSaveData con valores por defecto.
        SaveGame(); // Guarda la nueva partida en disco (si el modo es JSON).
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
        // Comprueba si los datos de guardado o los datos de combo no están inicializados.
        if (CurrentSave == null || CurrentSave.comboData == null)
        {
            Debug.LogWarning("No hay datos de guardado activos.");
            return; // Sale de la función si no hay datos de guardado.
        }

        // Obtiene la lista de combos equipados del CurrentSave.
        var combos = CurrentSave.comboData.equippedCombos;

        // Elimina cualquier combo anterior que exista para este tipo de arma.
        combos.RemoveAll(c => c.weaponType == weaponType);

        // Agrega el nuevo combo a la lista.
        combos.Add(new ComboSet
        {
            weaponType = weaponType,
            attackIDs = new List<int>(attackIDs) // Crea una nueva lista a partir de la lista proporcionada para evitar modificaciones externas.
        });

        SaveGame(); // Guarda los cambios en disco (si el modo es JSON).
        Debug.Log($"Combo equipado para {weaponType} con ataques: {string.Join(", ", attackIDs)}");
        // Ejemplo de uso (esto es un comentario, no se ejecuta):
        // PlayerSaveManager.Instance.EquipCombo(WeaponType.Claw, new List<int> { 1, 2, 3 });
    }

    public void UnlockAttack(int id, WeaponType category)
    {
        // Obtiene los datos de desbloqueo del CurrentSave.
        var data = CurrentSave.unlockData;

        // Utiliza la función AddAttackToDictionary para agregar el ID del ataque a la lista de ataques desbloqueados para su categoría.
        AddAttackToDictionary(data.unlockedAttacks, category, id);
    }

    public void PurchaseAttack(int id, WeaponType category)
    {
        // Obtiene los datos de posesión del CurrentSave.
        var data = CurrentSave.ownedData;

        // Utiliza la función AddAttackToDictionary para agregar el ID del ataque a la lista de ataques poseídos para su categoría.
        AddAttackToDictionary(data.ownedAttacks, category, id);
    }

    public bool IsAttackUnlocked(int id, WeaponType category)
    {
        // Intenta obtener la lista de ataques desbloqueados para la categoría dada.
        return CurrentSave.unlockData.unlockedAttacks.TryGetValue(category, out var list)
               // Si se encontró la lista y contiene el ID del ataque, retorna true.
               && list.Contains(id);
    }

    public bool IsAttackOwned(int id, WeaponType category)
    {
        // Intenta obtener la lista de ataques poseídos para la categoría dada.
        return CurrentSave.ownedData.ownedAttacks.TryGetValue(category, out var list)
               && list.Contains(id);
    }
}
