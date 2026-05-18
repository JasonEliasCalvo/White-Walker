using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatDatabase", menuName = "Combat Database")]
public class ActionDataSO : ScriptableObject
{
    public List<ActionData> allAttacks;
}