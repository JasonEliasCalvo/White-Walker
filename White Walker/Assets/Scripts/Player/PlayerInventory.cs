using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Soma/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    public int gold = 500;

    [Tooltip("Todos los ataques desbloqueados (aunque no estén comprados aún)")]
    public List<AttackBase> unlockedAttacks = new();

    [Tooltip("Ataques que el jugador ya compró")]
    public List<AttackBase> ownedAttacks = new();

    [Header("Combos Equipados")]
    public List<int> comboClaw = new();
    public List<int> comboSword = new();
    public List<int> comboGun = new();

    public bool IsUnlocked(AttackBase attack) => unlockedAttacks.Contains(attack);
    public bool IsOwned(AttackBase attack) => ownedAttacks.Contains(attack);

    public delegate void InventoryEvent(AttackBase attack);
    public InventoryEvent eventAttackUnlocked;
    public InventoryEvent eventAttackPurchased;

    public void Unlock(AttackBase attack)
    {
        if (!unlockedAttacks.Contains(attack))
        {
            unlockedAttacks.Add(attack);
            eventAttackUnlocked?.Invoke(attack);
        }
    }

    public void Purchase(AttackBase attack)
    {
        if (!ownedAttacks.Contains(attack))
        {
            ownedAttacks.Add(attack);
            eventAttackPurchased?.Invoke(attack); 
        }
    }
}
