using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Soma/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    public int gold = 500;

    [Tooltip("Todos los ataques desbloqueados (aunque no estén comprados aún)")]
    public List<AttackData> unlockedAttacks = new();

    [Tooltip("Ataques que el jugador ya compró")]
    public List<AttackData> ownedAttacks = new();

    [Header("Combos Equipados")]
    public List<int> comboClaw = new();
    public List<int> comboSword = new();

    public bool IsUnlocked(AttackData attack) => unlockedAttacks.Contains(attack);
    public bool IsOwned(AttackData attack) => ownedAttacks.Contains(attack);

    public delegate void InventoryEvent(AttackData attack);
    public InventoryEvent eventAttackUnlocked;
    public InventoryEvent eventAttackPurchased;

    public void Unlock(AttackData attack)
    {
        if (!unlockedAttacks.Contains(attack))
        {
            unlockedAttacks.Add(attack);
            eventAttackUnlocked?.Invoke(attack);
        }
    }

    public void Purchase(AttackData attack)
    {
        if (!ownedAttacks.Contains(attack))
        {
            ownedAttacks.Add(attack);
            eventAttackPurchased?.Invoke(attack); 
        }
    }
}
