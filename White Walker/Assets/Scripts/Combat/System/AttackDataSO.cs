using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatDatabase", menuName = "Combat Database")]
public class AttackDataSO : ScriptableObject
{
    public List<AttackBase> allAttacks;
}