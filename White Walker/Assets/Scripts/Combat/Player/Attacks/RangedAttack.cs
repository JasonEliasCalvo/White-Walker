using UnityEngine;

[CreateAssetMenu(fileName = "NewRangedAttack", menuName = "Soma/RangedAttack")]
public class RangedAttack : AttackBase
{
    public GameObject projectilePrefab;   
    public Vector3 fireOffset;
}
