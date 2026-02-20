using System.Collections;
using UnityEngine;
public class AttackGhost : MonoBehaviour
{
    public AttackBase attackData;
    public Animator animator;
    public float selfDestructDelay = 2f;

    public void Init(AttackBase atk)
    {
        attackData = atk;
        animator = GetComponentInChildren<Animator>();
        animator.Play(atk.animation.name);
        StartCoroutine(DestroyAfterDelay(atk.TotalDuration));
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

