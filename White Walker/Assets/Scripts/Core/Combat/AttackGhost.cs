using System.Collections;
using UnityEngine;
public class AttackGhost : MonoBehaviour
{
    public ActionData attackData;
    public Animator animator;
    public float selfDestructDelay = 2f;

    public void Init(ActionData atk)
    {
        attackData = atk;
        animator = GetComponentInChildren<Animator>();
        animator.Play(atk.animation.name);
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

