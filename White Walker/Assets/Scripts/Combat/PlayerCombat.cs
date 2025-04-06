using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float stunDuration = 2f;
    [SerializeField] private float downDuration = 3f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask layerMaskDetec;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryAttack();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            DoParry();
        }
    }

    public void TryAttack()
    {
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, layerMaskDetec);

        foreach (Collider hit in hits)
        {
            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(attackDamage);
                Debug.Log("Golpeaste a: " + hit.name);

                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    if (enemy.IsStunned)
                    {
                        Debug.Log("Ataque especial contra enemigo aturdido.");
                        enemy.KnockDown(downDuration);
                    }
                    else if (enemy.IsDowned)
                    {
                        Debug.Log("Remate en el suelo.");
                        enemy.TakeDamage(attackDamage * 2);
                    }
                    else
                    {
                        enemy.Stun(stunDuration);
                    }
                }
                break;
            }
        }
    }

    private void DoParry()
    {
        Debug.Log("Intentaste hacer un parry.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
