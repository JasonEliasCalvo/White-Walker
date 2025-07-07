using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] private AttackDataSO attackDataSO;

    private Dictionary<int, AttackBase> attackById = new();
    private Dictionary<StyleType, List<AttackBase>> attacksByCategory = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        attackById.Clear();
        attacksByCategory.Clear();

        foreach (var attack in attackDataSO.allAttacks)
        {
            if (attackById.ContainsKey(attack.attackID))
            {
                Debug.LogWarning($"Duplicate attack ID: {attack.attackID} in {attack.attackName}");
                continue;
            }

            attackById.Add(attack.attackID, attack);

            if (!attacksByCategory.ContainsKey(attack.category))
                attacksByCategory[attack.category] = new List<AttackBase>();

            attacksByCategory[attack.category].Add(attack);
        }
    }

    public AttackBase GetAttackById(int id)
    {
        attackById.TryGetValue(id, out var attack);
        return attack;
    }

    public List<AttackBase> GetAttacksByCategory(StyleType category)
    {
        attacksByCategory.TryGetValue(category, out var list);
        return list;
    }

    public List<AttackBase> GetAllAttacks()
    {
        return attackById.Values.ToList();
    }

    public List<AttackBase> GetUnlockedAttacks(StyleType category)
    {
        var ids = PlayerSaveManager.Instance.CurrentSave.unlockData
            .unlockedAttacks.TryGetValue(category, out var list) ? list : new List<int>();

        return GetAttacksByCategory(category)
            .Where(a => ids.Contains(a.attackID))
            .ToList();
    }
}
