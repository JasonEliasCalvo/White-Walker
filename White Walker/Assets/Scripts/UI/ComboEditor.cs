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
            ownedAttacksByType[type] = playerInventory.ownedAttacks
                .Where(a => a.category == type)
                .ToList();
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
        var save = PlayerSaveManager.Instance.CurrentSave.comboData;

        save.equippedCombos[WeaponType.Claw] = GetIDsFromDropdowns(claw1, claw2, claw3);
        save.equippedCombos[WeaponType.Sword] = GetIDsFromDropdowns(sword1, sword2, sword3);
        save.equippedCombos[WeaponType.Gun] = GetIDsFromDropdowns(gun);

        PlayerSaveManager.Instance.SaveGame();
        Debug.Log("Combos guardados");
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
        foreach (WeaponType type in ownedAttacksByType.Keys)
        {
            if (!PlayerSaveManager.Instance.CurrentSave.comboData.equippedCombos.TryGetValue(type, out var saved)) continue;

            var dropdowns = GetDropdownsByWeapon(type);
            for (int i = 0; i < dropdowns.Length && i < saved.Count; i++)
            {
                int id = saved[i];
                var index = ownedAttacksByType[type].FindIndex(a => a.attackID == id);
                dropdowns[i].value = index >= 0 ? index + 1 : 0; // +1 porque "(Ninguno)" está en el index 0
            }
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
}
