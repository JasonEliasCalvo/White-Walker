using System.Collections.Generic;
using UnityEngine;

public class AttackShopPanel : MonoBehaviour
{
    public static AttackShopPanel Instance;

    [SerializeField] private GameObject attackSlotPrefab;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private CombatData combatData;
    [SerializeField] private CombatDatabaseSO database;
    [SerializeField] private ComboEditorPanel comboEditor;


    private void Awake()
    {
        Instance = this;
    }

    public void ShowShop(AttackCategory category)
    {
        gameObject.SetActive(true);
        ClearSlots();

        List<AttackBase> attacks = GetAttacksByCategory(category);

        foreach (var attack in attacks)
        {
            GameObject slotObj = Instantiate(attackSlotPrefab, contentPanel);
            AttackSlotUI slot = slotObj.GetComponent<AttackSlotUI>();
            bool isUnlocked = combatData.IsUnlocked(attack.id, category);

            slot.Setup(attack, isUnlocked, () =>
            {
                if (isUnlocked)
                {
                    BuyAttack(attack, category);
                }
            });
        }
    }

    private void BuyAttack(AttackBase attack, AttackCategory category)
    {
        combatData.UnlockAttack(attack.id, category);
        combatData.SaveUnlockedAttacks();

        bool isUnlocked = combatData.IsUnlocked(attack.id, category);

        // Aquí iría un panel de confirmación real
        bool equipNow = true; // Simulamos respuesta positiva

        if (equipNow)
        {
            gameObject.SetActive(false);
            comboEditor.OpenEditor(category, combatData);
        }
    }

    private List<AttackBase> GetAttacksByCategory(AttackCategory category)
    {
        switch (category)
        {
            case AttackCategory.FistKick:
                return database.fistKickAttacks.ConvertAll<AttackBase>(x => x);
            case AttackCategory.Dagger:
                return database.daggerAttacks.ConvertAll<AttackBase>(x => x);
            case AttackCategory.Gun:
                return database.gunAttacks.ConvertAll<AttackBase>(x => x);
            case AttackCategory.Finisher:
                return database.finishers.ConvertAll<AttackBase>(x => x);
            default:
                return new List<AttackBase>();
        }
    }

    private void ClearSlots()
    {
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);
    }
}
