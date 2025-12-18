using UnityEngine;

public class EnemyController : HealthSystem
{
    public EnemyState currentState = EnemyState.Normal;

    public bool IsStunned => currentState == EnemyState.Stunned;
    public bool IsDowned => currentState == EnemyState.Downed;

    protected override void Die()
    {
        base.Die();
        // Muerte personalizada del enemigo
        Destroy(gameObject);
    }

    public void Stun(float duration)
    {
        if (IsDowned) return;
        currentState = EnemyState.Stunned;
        Debug.Log("Enemigo aturdido.");

        Invoke(nameof(RecoverFromStun), duration);
    }

    public void KnockDown(float duration)
    {
        currentState = EnemyState.Downed;
        Debug.Log("Enemigo derribado.");

        Invoke(nameof(RecoverFromDown), duration);
    }

    private void RecoverFromStun()
    {
        currentState = EnemyState.Normal;
        Debug.Log("Enemigo se recuperó del aturdimiento.");
    }

    private void RecoverFromDown()
    {
        currentState = EnemyState.Normal;
        Debug.Log("Enemigo se levantó.");
    }

}
