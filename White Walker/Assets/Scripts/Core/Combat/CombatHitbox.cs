using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHitbox : MonoBehaviour
{
    private float damage;
    private float hitStun;

    private FighterEntity owner;
    private Collider myCollider;
    private List<IDamageable> victims = new List<IDamageable>();
    private ActionData attackData;

    void Awake()
    {
        myCollider = GetComponent<Collider>();
        owner = GetComponentInParent<FighterEntity>();
        myCollider.enabled = false;
    }

    public void EnableHitbox(float dmg, float stun, float force, ActionData attack = null)
    {
        damage = dmg;
        hitStun = stun;

        attackData = attack;

        victims.Clear();
        myCollider.enabled = true;
    }

    public void DisableHitbox()
    {
        myCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (owner != null && other.gameObject == owner.gameObject) return;

        IDamageable target = other.GetComponent<IDamageable>();

        if (target == null)
            return;


        if (victims.Contains(target))
            return;

        victims.Add(target);

        Vector3 hitPoint = myCollider.ClosestPoint(
       other.bounds.center);

        if (attackData != null)
        {
            Debug.Log(
                $"<color=yellow>HIT:</color> " +
                $"{owner.gameObject.name} -> {other.gameObject.name}"
            );

            Debug.Log(
                $"<color=lime>CREANDO PARTICULA</color> " +
                $"Ataque: {attackData.actionName} | " +
                $"Prefab: {attackData.hitParticle.name} | " +
                $"Posición: {hitPoint}"
            );

            Debug.Log(
                $"Ataque: {attackData.actionName}"
            );

            if (attackData.hitParticle != null)
            {
                Debug.Log(
                    $"PARTICULA: {attackData.hitParticle.name}"
                );

                Instantiate(
                    attackData.hitParticle,
                    hitPoint,
                    Quaternion.identity
                );
            }
            else
            {
                Debug.LogError(
                    $"El ataque {attackData.actionName} NO tiene hitParticle."
                );
            }

            if (attackData.hitSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    attackData.hitSound,
                    hitPoint
                );
            }
        }
        else
        {
            Debug.LogError(
                $"attackData es NULL en {owner.gameObject.name}"
            );
        }

        // 3. APLICAR DAÑO
        target.TakeDamage(damage, hitStun);
    }
}
