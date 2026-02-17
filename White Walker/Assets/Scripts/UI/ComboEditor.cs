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

    private Dictionary<StyleType, List<AttackBase>> ownedAttacksByType;

    private void Start()
    {
        LoadAttacks();
        PopulateDropdowns();
        LoadSavedCombos();
    }

    void LoadAttacks()
    {
        ownedAttacksByType = new();

        foreach (StyleType type in System.Enum.GetValues(typeof(StyleType)))
        {
            ownedAttacksByType[type] = GetOwnedAttacksByType(type);
        }
    }

    List<AttackBase> GetOwnedAttacksByType(StyleType type)
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

    #region Dropdown Population
    void PopulateDropdowns()
    {
        PopulateDropdownsForWeapon(StyleType.Unarmed, GetDropdownsByWeapon(StyleType.Unarmed));
        PopulateDropdownsForWeapon(StyleType.Armed, GetDropdownsByWeapon(StyleType.Armed));
    }

    void PopulateDropdownsForWeapon(StyleType weaponType, TMP_Dropdown[] dropdowns)
    {
        foreach (var dropdown in dropdowns)
        {
            SetupDropdown(dropdown, weaponType);
        }
    }


    void SetupDropdown(TMP_Dropdown dropdown, StyleType type)
    {
        dropdown.ClearOptions();
        if (ownedAttacksByType.TryGetValue(type, out var attacks))
        {
            List<string> options = attacks.Select(a => a.attackName).ToList();
            options.Insert(0, "(Ninguno)");
            dropdown.AddOptions(options);
        }
    }

    #endregion

    StyleType GetWeaponTypeFromDropdown(TMP_Dropdown dropdown)
    {
        if (dropdown == claw1 || dropdown == claw2 || dropdown == claw3) return StyleType.Unarmed;
        if (dropdown == sword1 || dropdown == sword2 || dropdown == sword3) return StyleType.Armed;
        else return StyleType.Unarmed; // Default, aunque no debería pasar
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
            save.equippedCombos.RemoveAll(c => c.weaponType == StyleType.Unarmed || c.weaponType == StyleType.Armed);

            // Guardamos los combos nuevos
            save.equippedCombos.Add(new ComboSet { weaponType = StyleType.Unarmed, attackIDs = clawIDs });
            save.equippedCombos.Add(new ComboSet { weaponType = StyleType.Armed, attackIDs = swordIDs });

            PlayerSaveManager.Instance.SaveGame();
            Debug.Log("Combos guardados en JSON");
        }
        else // ScriptableObject
        {
            playerInventory.comboClaw = clawIDs;
            playerInventory.comboSword = swordIDs;

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
                if (ownedAttacksByType.TryGetValue(type, out var list) && i < list.Count)
                {
                    ids.Add(list[i].attackID);
                }
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
            ApplySavedCombo(StyleType.Unarmed, playerInventory.comboClaw, new[] { claw1, claw2, claw3 });
            ApplySavedCombo(StyleType.Armed, playerInventory.comboSword, new[] { sword1, sword2, sword3 });
        }
    }

    void ApplySavedCombo(StyleType type, List<int> ids, TMP_Dropdown[] dropdowns)
    {
        for (int i = 0; i < dropdowns.Length && i < ids.Count; i++)
        {
            int id = ids[i];
            var index = ownedAttacksByType[type].FindIndex(a => a.attackID == id);
            dropdowns[i].value = index >= 0 ? index + 1 : 0;
        }
    }

    TMP_Dropdown[] GetDropdownsByWeapon(StyleType type)
    {
        return type switch
        {
            StyleType.Unarmed => new[] { claw1, claw2, claw3 },
            StyleType.Armed => new[] { sword1, sword2, sword3 },
            _ => new TMP_Dropdown[0]
        };
    }

    public void ResetToDefaultCombos()
    {
        Debug.Log("Reseteando combos a los valores por defecto...");

        ApplySavedCombo(StyleType.Unarmed, defaultClawCombo, GetDropdownsByWeapon(StyleType.Unarmed));
        ApplySavedCombo(StyleType.Armed, defaultSwordCombo, GetDropdownsByWeapon(StyleType.Armed));

        SaveCombo();
    }
}
