using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ComboEditorPanel : MonoBehaviour
{
    public static ComboEditorPanel Instance;

    [SerializeField] private GameObject attackSlotPrefab;
    [SerializeField] private Transform comboSlotsHolder;
    [SerializeField] private CombatDatabaseSO database;

    private CombatData combatData;
    private PlayerSaveData playerData;

    private void Awake() => Instance = this;

    public void OpenEditor(AttackCategory category, CombatData data)
    {
        combatData = data;
        gameObject.SetActive(true);
        LoadAvailableAttacks(category);
        // Puedes guardar `newAttackID` si quieres auto asignarlo en el combo
    }

    private void LoadAvailableAttacks(AttackCategory category)
    {
        // Limpia los slots actuales
        foreach (Transform child in comboSlotsHolder)
            Destroy(child.gameObject);

        // Obtenemos todos los ataques según categoría
        List<AttackBase> attacks = category switch
        {
            AttackCategory.FistKick => database.fistKickAttacks.Cast<AttackBase>().ToList(),
            AttackCategory.Dagger => database.daggerAttacks.Cast<AttackBase>().ToList(),
            AttackCategory.Gun => database.gunAttacks.Cast<AttackBase>().ToList(),
            AttackCategory.Finisher => database.finishers.Cast<AttackBase>().ToList(),
            _ => new List<AttackBase>(),
        };

        foreach (var atk in attacks)
        {
            if (combatData.IsUnlocked(atk.id, category))
            {
                var go = Instantiate(attackSlotPrefab, comboSlotsHolder);
                go.GetComponent<ComboSlot>().Init(atk);
            }
        }
    }

    void SaveCombo(AttackCategory category, List<AttackBase> selectedAttacks)
    {
        List<int> ids = selectedAttacks.Select(a => a.id).ToList();
        playerData.comboData.equippedCombos[category] = ids;

        SaveSystem.Save(playerData);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
