using UnityEngine;

public class Bullet : MonoBehaviour
{
    public AttackBase attackData;
    public float speed = 10f;

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(attackData.damage);

            var enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                if (AttackUtils.HasEffect(attackData, EffectTag.Stun))
                    enemy.Stun(AttackUtils.GetEffectDuration(attackData, EffectTag.Stun));

                if (AttackUtils.HasEffect(attackData, EffectTag.Knockdown))
                    enemy.KnockDown(AttackUtils.GetEffectDuration(attackData, EffectTag.Knockdown));
            }

            Destroy(gameObject);
        }
    }
}
