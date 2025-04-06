using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatDatabase", menuName = "Combat/Database")]
public class CombatDatabaseSO : ScriptableObject
{
    public List<AttackBase> allAttacks;
    public List<FistKickAttack> fistKickAttacks;
    public List<DaggerAttack> daggerAttacks;
    public List<GunAttack> gunAttacks;
    public List<Finisher> finishers;
    public List<Taunt> taunts;

    public void UpdateAllAttacks()
    {
        allAttacks = new();
        allAttacks.AddRange(fistKickAttacks);
        allAttacks.AddRange(daggerAttacks);
        allAttacks.AddRange(gunAttacks);
        allAttacks.AddRange(finishers);
    }
}