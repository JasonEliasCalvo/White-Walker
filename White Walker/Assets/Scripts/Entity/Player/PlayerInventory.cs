using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Soma/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    public int gold = 500;

    [Tooltip("Todos los ataques desbloqueados (aunque no estén comprados aún)")]
    public List<ActionData> unlockedAttacks = new();

    [Tooltip("Ataques que el jugador ya compró")]
    public List<ActionData> ownedAttacks = new();

    [Header("Combos Equipados")]
    public List<int> comboClaw = new();
    public List<int> comboSword = new();

    public bool IsUnlocked(ActionData attack) => unlockedAttacks.Contains(attack);
    public bool IsOwned(ActionData attack) => ownedAttacks.Contains(attack);

    public delegate void InventoryEvent(ActionData attack);
    public InventoryEvent eventAttackUnlocked;
    public InventoryEvent eventAttackPurchased;

    public void Unlock(ActionData attack)
    {
        if (!unlockedAttacks.Contains(attack))
        {
            unlockedAttacks.Add(attack);
            eventAttackUnlocked?.Invoke(attack);
        }
    }

    public void Purchase(ActionData attack)
    {
        if (!ownedAttacks.Contains(attack))
        {
            ownedAttacks.Add(attack);
            eventAttackPurchased?.Invoke(attack); 
        }
    }
}
