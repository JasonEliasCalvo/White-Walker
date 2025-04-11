using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Prefabs y UI")]
    public Transform attackListContainer;
    public GameObject attackButtonPrefab;
    public AttackDetailPanelUI detailPanel;
    public GameObject equipConfirmPanel;
    public GameObject comboEditorPanel;

    [Header("Filtros")]
    public WeaponType currentCategory;
    public Button clawButton;
    public Button swordButton;
    public Button gunButton;

    [Header("Datos")]
    public PlayerInventory playerInventory;

    private void Start()
    {
        clawButton.onClick.AddListener(() => ChangeCategory(WeaponType.Claw));
        swordButton.onClick.AddListener(() => ChangeCategory(WeaponType.Sword));
        gunButton.onClick.AddListener(() => ChangeCategory(WeaponType.Gun));

        if (PlayerSaveManager.Instance.currentMode == SaveMode.Scriptable)
        {
            playerInventory.eventAttackUnlocked += OnInventoryUpdated;
            playerInventory.eventAttackPurchased += OnInventoryUpdated;
        }

        PopulateShop();
    }


    private void ChangeCategory(WeaponType newCategory)
    {
        currentCategory = newCategory;
        PopulateShop();
    }

    void OnInventoryUpdated(AttackBase updatedAttack)
    {
        PopulateShop();
    }

    void PopulateShop()
    {
        foreach (Transform child in attackListContainer)
            Destroy(child.gameObject);

        List<AttackBase> attacks = CombatManager.Instance.GetAttacksByCategory(currentCategory);

        foreach (var attack in attacks)
        {
            GameObject buttonTemp = Instantiate(attackButtonPrefab, attackListContainer);
            AttackButtonUI buttonUI = buttonTemp.GetComponent<AttackButtonUI>();

            bool unlocked = playerInventory.IsUnlocked(attack);
            bool owned = playerInventory.IsOwned(attack);

            buttonUI.Setup(attack, unlocked, owned, a =>
            detailPanel.ShowAttack(a, unlocked, owned, this));
        }

        detailPanel.HidePanel();
    }

    public void TryBuyAttack(AttackBase attack)
    {
        if (!IsUnlocked(attack)) return;

        if (GetGold() < attack.cost)
        {
            Debug.Log("No tienes suficiente oro.");
            return;
        }

        SpendGold(attack.cost);

        if (PlayerSaveManager.Instance.currentMode == SaveMode.Scriptable)
        {
            playerInventory.Purchase(attack);
        }
        else
        {
            PlayerSaveManager.Instance.UnlockAttack(attack.attackID, attack.category);
            PlayerSaveManager.Instance.PurchaseAttack(attack.attackID, attack.category);
            PlayerSaveManager.Instance.SaveGame();
        }

        ShowEquipConfirmation(attack);
    }

    private void ShowEquipConfirmation(AttackBase purchasedAttack)
    {
        equipConfirmPanel.SetActive(true);

        Button[] buttons = equipConfirmPanel.GetComponentsInChildren<Button>();
        buttons[0].onClick.RemoveAllListeners();
        buttons[1].onClick.RemoveAllListeners();

        buttons[0].onClick.AddListener(() =>
        {
            comboEditorPanel.SetActive(true);
            equipConfirmPanel.SetActive(false);
            gameObject.SetActive(false);
        });

        buttons[1].onClick.AddListener(() =>
        {
            equipConfirmPanel.SetActive(false);
        });
    }

    bool IsUnlocked(AttackBase attack)
    {
        if (PlayerSaveManager.Instance.currentMode == SaveMode.Scriptable)
            return playerInventory.IsUnlocked(attack);

        var save = PlayerSaveManager.Instance.CurrentSave;
        return save.unlockData.unlockedAttacks.TryGetValue(attack.category, out var list)
               && list.Contains(attack.attackID);
    }

    bool IsOwned(AttackBase attack)
    {
        if (PlayerSaveManager.Instance.currentMode == SaveMode.Scriptable)
            return playerInventory.IsOwned(attack);

        return PlayerSaveManager.Instance.IsAttackOwned(attack.attackID, attack.category);
    }

    int GetGold()
    {
        if (PlayerSaveManager.Instance.currentMode == SaveMode.Scriptable)
            return playerInventory.gold;

        return PlayerSaveManager.Instance.CurrentSave.playerStats.money;
    }

    void SpendGold(int amount)
    {
        if (PlayerSaveManager.Instance.currentMode == SaveMode.Scriptable)
            playerInventory.gold -= amount;
        else
            PlayerSaveManager.Instance.CurrentSave.playerStats.money -= amount;
    }
}
