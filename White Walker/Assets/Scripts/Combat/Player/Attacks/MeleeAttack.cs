using UnityEngine;

[CreateAssetMenu(fileName = "New Melee", menuName = "Combat/MeleeAttack")]
public class MeleeAttack : AttackBase
{
    [Header("Combo System")]
    public MeleeAttack nextAttack;
}
