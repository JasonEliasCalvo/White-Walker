using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ComboEditor : MonoBehaviour
{
    [Header("Dropdowns Garras")]
    public TMP_Dropdown claw1;
    public TMP_Dropdown claw2;
    public TMP_Dropdown claw3;

    [Header("Dropdowns Espada")]
    public TMP_Dropdown sword1;
    public TMP_Dropdown sword2;
    public TMP_Dropdown sword3;

    [Header("Dropdown Pistola")]
    public TMP_Dropdown gun;

    [Header("Datos")]
    public PlayerInventory playerInventory;

    [Header("Combos por defecto (IDs)")]
    public List<int> defaultClawCombo = new();
    public List<int> defaultSwordCombo = new();
    public List<int> defaultGunCombo = new();

    private Dictionary<WeaponType, List<AttackBase>> ownedAttacksByType;

    private void Start()
    {
        LoadAttacks();
        PopulateDropdowns();
        LoadSavedCombos();
    }

    void LoadAttacks()
    {
        ownedAttacksByType = new();

        foreach (WeaponType type in System.Enum.GetValues(typeof(WeaponType)))
        {
            ownedAttacksByType[type] = GetOwnedAttacksByType(type);
        }
    }

    List<AttackBase> GetOwnedAttacksByType(WeaponType type)
    {
        if (PlayerSaveManager.Instance.currentMode == SaveMode.Scriptable)
        {
            return playerInventory.ownedAttacks
                .Where(a => a.category == type)
                .ToList();
        }
        else // JSON
        {
            var save = PlayerSaveManager.Instance.CurrentSave;
            if (save.unlockData.unlockedAttacks.TryGetValue(type, out var ids))
            {
                return CombatManager.Instance.GetAttacksByCategory(type)
                    .Where(a => ids.Contains(a.attackID))
                    .ToList();
            }
            return new List<AttackBase>();
        }
    }

    void PopulateDropdowns()
    {
        SetupDropdown(claw1, WeaponType.Claw, GetUsedAttackIDs(claw2, claw3));
        SetupDropdown(claw2, WeaponType.Claw, GetUsedAttackIDs(claw1, claw3));
        SetupDropdown(claw3, WeaponType.Claw, GetUsedAttackIDs(claw1, claw2));

        SetupDropdown(sword1, WeaponType.Sword, GetUsedAttackIDs(sword2, sword3));
        SetupDropdown(sword2, WeaponType.Sword, GetUsedAttackIDs(sword1, sword3));
        SetupDropdown(sword3, WeaponType.Sword, GetUsedAttackIDs(sword1, sword2));

        SetupDropdown(gun, WeaponType.Gun, new List<int>());
    }

    void SetupDropdown(TMP_Dropdown dropdown, WeaponType type, List<int> excludeIDs)
    {
        dropdown.ClearOptions();
        var attacks = ownedAttacksByType[type]
            .Where(a => !excludeIDs.Contains(a.attackID))
            .ToList();

        List<string> options = attacks.Select(a => a.attackName).ToList();
        options.Insert(0, "(Ninguno)");

        dropdown.AddOptions(options);
    }

    List<int> GetUsedAttackIDs(params TMP_Dropdown[] others)
    {
        List<int> ids = new();

        foreach (var dd in others)
        {
            int i = dd.value - 1;
            if (i >= 0)
            {
                var list = ownedAttacksByType[GetWeaponTypeFromDropdown(dd)];
                if (i < list.Count)
                    ids.Add(list[i].attackID);
            }
        }

        return ids;
    }

    WeaponType GetWeaponTypeFromDropdown(TMP_Dropdown dd)
    {
        if (dd == claw1 || dd == claw2 || dd == claw3) return WeaponType.Claw;
        if (dd == sword1 || dd == sword2 || dd == sword3) return WeaponType.Sword;
        return WeaponType.Gun;
    }

    public void SaveCombo()
    {
        var clawIDs = GetIDsFromDropdowns(claw1, claw2, claw3);
        var swordIDs = GetIDsFromDropdowns(sword1, sword2, sword3);
        var gunIDs = GetIDsFromDropdowns(gun);

        if (PlayerSaveManager.Instance.currentMode == SaveMode.Json)
        {
            var save = PlayerSaveManager.Instance.CurrentSave.comboData;

            // Eliminamos entradas anteriores
            save.equippedCombos.RemoveAll(c => c.weaponType == WeaponType.Claw || c.weaponType == WeaponType.Sword || c.weaponType == WeaponType.Gun);

            // Guardamos los combos nuevos
            save.equippedCombos.Add(new ComboSet { weaponType = WeaponType.Claw, attackIDs = clawIDs });
            save.equippedCombos.Add(new ComboSet { weaponType = WeaponType.Sword, attackIDs = swordIDs });
            save.equippedCombos.Add(new ComboSet { weaponType = WeaponType.Gun, attackIDs = gunIDs });

            PlayerSaveManager.Instance.SaveGame();
            Debug.Log("Combos guardados en JSON");
        }
        else // ScriptableObject
        {
            playerInventory.comboClaw = clawIDs;
            playerInventory.comboSword = swordIDs;
            playerInventory.comboGun = gunIDs;

            Debug.Log("Combos guardados en ScriptableObject");
        }

        ComboEvents.OnComboChanged?.Invoke();
    }

    List<int> GetIDsFromDropdowns(params TMP_Dropdown[] dropdowns)
    {
        List<int> ids = new();
        foreach (var dd in dropdowns)
        {
            int i = dd.value - 1;
            if (i >= 0)
            {
                var type = GetWeaponTypeFromDropdown(dd);
                var list = ownedAttacksByType[type];
                if (i < list.Count)
                    ids.Add(list[i].attackID);
            }
        }
        return ids;
    }

    void LoadSavedCombos()
    {
        if (PlayerSaveManager.Instance.currentMode == SaveMode.Json)
        {
            var comboData = PlayerSaveManager.Instance.CurrentSave.comboData;
            foreach (var comboSet in comboData.equippedCombos)
            {
                var type = comboSet.weaponType;
                if (!ownedAttacksByType.ContainsKey(type)) continue;

                var dropdowns = GetDropdownsByWeapon(type);
                for (int i = 0; i < dropdowns.Length && i < comboSet.attackIDs.Count; i++)
                {
                    int id = comboSet.attackIDs[i];
                    int index = ownedAttacksByType[type].FindIndex(a => a.attackID == id);
                    dropdowns[i].value = index >= 0 ? index + 1 : 0;
                }
            }
        }
        else
        {
            ApplySavedCombo(WeaponType.Claw, playerInventory.comboClaw, new[] { claw1, claw2, claw3 });
            ApplySavedCombo(WeaponType.Sword, playerInventory.comboSword, new[] { sword1, sword2, sword3 });
            ApplySavedCombo(WeaponType.Gun, playerInventory.comboGun, new[] { gun });
        }
    }

    void ApplySavedCombo(WeaponType type, List<int> ids, TMP_Dropdown[] dropdowns)
    {
        for (int i = 0; i < dropdowns.Length && i < ids.Count; i++)
        {
            int id = ids[i];
            var index = ownedAttacksByType[type].FindIndex(a => a.attackID == id);
            dropdowns[i].value = index >= 0 ? index + 1 : 0;
        }
    }

    TMP_Dropdown[] GetDropdownsByWeapon(WeaponType type)
    {
        return type switch
        {
            WeaponType.Claw => new[] { claw1, claw2, claw3 },
            WeaponType.Sword => new[] { sword1, sword2, sword3 },
            WeaponType.Gun => new[] { gun },
            _ => new TMP_Dropdown[0]
        };
    }

    public void ResetToDefaultCombos()
    {
        Debug.Log("Reseteando combos a los valores por defecto...");

        ApplySavedCombo(WeaponType.Claw, defaultClawCombo, GetDropdownsByWeapon(WeaponType.Claw));
        ApplySavedCombo(WeaponType.Sword, defaultSwordCombo, GetDropdownsByWeapon(WeaponType.Sword));
        ApplySavedCombo(WeaponType.Gun, defaultGunCombo, GetDropdownsByWeapon(WeaponType.Gun));

        SaveCombo(); // También los guarda
    }
}
