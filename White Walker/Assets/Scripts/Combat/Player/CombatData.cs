using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AttackCategory
{
    FistKick,
    Dagger,
    SpecialDagger,
    Gun,
    Finisher
}

public class CombatData : MonoBehaviour
{
    [Header("Combat Database")]
    [SerializeField] private CombatDatabaseSO database;

    private Dictionary<int, FistKickAttack> fistKickDict;
    private Dictionary<int, DaggerAttack> daggerDict;
    private Dictionary<int, SpecialDaggerAttack> specialDaggerDict;
    private Dictionary<int, GunAttack> gunDict;
    private Dictionary<int, Finisher> finisherDict;

    private HashSet<int> unlockedFistKick = new();
    private HashSet<int> unlockedDagger = new();
    private HashSet<int> unlockedSpecialDagger = new();
    private HashSet<int> unlockedGun = new();
    private HashSet<int> unlockedFinisher = new();

    private void Awake()
    {
        if (database == null)
        {
            Debug.LogError("CombatDatabaseSO is not assigned.");
            return;
        }

        fistKickDict = CreateDict(database.fistKickAttacks);
        daggerDict = CreateDict(database.daggerAttacks);
        specialDaggerDict = CreateDict(database.specialDaggerAttacks);
        gunDict = CreateDict(database.gunAttacks);
        finisherDict = CreateDict(database.finishers);

        LoadUnlockedAttacks();
    }

    private void LoadUnlockedAttacks()
    {
        var data = CombatSaveSystem.LoadData();

        unlockedFistKick = new(data.unlockedFistKick);
        unlockedDagger = new(data.unlockedDagger);
        unlockedSpecialDagger = new(data.unlockedSpecialDagger);
        unlockedGun = new(data.unlockedGun);
        unlockedFinisher = new(data.unlockedFinishers);
    }

    public void SaveUnlockedAttacks()
    {
        var data = new CombatSaveData
        {
            unlockedFistKick = unlockedFistKick.ToList(),
            unlockedDagger = unlockedDagger.ToList(),
            unlockedSpecialDagger = unlockedSpecialDagger.ToList(),
            unlockedGun = unlockedGun.ToList(),
            unlockedFinishers = unlockedFinisher.ToList()
        };

        CombatSaveSystem.SaveData(data);
    }

    private Dictionary<int, T> CreateDict<T>(List<T> list) where T : AttackBase
    {
        Dictionary<int, T> dict = new();
        foreach (var atk in list)
            dict[atk.id] = atk;

        return dict;
    }

    // Getters
    public FistKickAttack GetFistKickAttack(int id) => fistKickDict.TryGetValue(id, out var atk) ? atk : null;
    public DaggerAttack GetDaggerAttack(int id) => daggerDict.TryGetValue(id, out var atk) ? atk : null;
    public SpecialDaggerAttack GetSpecialDaggerAttack(int id) => specialDaggerDict.TryGetValue(id, out var atk) ? atk : null;
    public GunAttack GetGunAttack(int id) => gunDict.TryGetValue(id, out var atk) ? atk : null;
    public Finisher GetFinisher(int id) => finisherDict.TryGetValue(id, out var atk) ? atk : null;

    // Unlock
    public void UnlockAttack(int id, AttackCategory category)
    {
        switch (category)
        {
            case AttackCategory.FistKick: unlockedFistKick.Add(id); break;
            case AttackCategory.Dagger: unlockedDagger.Add(id); break;
            case AttackCategory.SpecialDagger: unlockedSpecialDagger.Add(id); break;
            case AttackCategory.Gun: unlockedGun.Add(id); break;
            case AttackCategory.Finisher: unlockedFinisher.Add(id); break;
        }
    }

    public bool IsUnlocked(int id, AttackCategory category)
    {
        return category switch
        {
            AttackCategory.FistKick => unlockedFistKick.Contains(id),
            AttackCategory.Dagger => unlockedDagger.Contains(id),
            AttackCategory.SpecialDagger => unlockedSpecialDagger.Contains(id),
            AttackCategory.Gun => unlockedGun.Contains(id),
            AttackCategory.Finisher => unlockedFinisher.Contains(id),
            _ => false,
        };
    }
}
