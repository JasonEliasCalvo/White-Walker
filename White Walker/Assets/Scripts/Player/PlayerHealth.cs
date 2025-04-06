using UnityEngine;

public class PlayerHealth : HealthSystem
{
    protected override void Die()
    {
        base.Die();
        Debug.Log("¡El jugador ha muerto!");
        // Aquí puedes poner animaciones, UI de game over, etc.

        // GameManager.Instance.TriggerGameOver();
    }
    private void Update()
    {
        // Debug para probar daño y curación con teclas
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
            Debug.Log($"Jugador recibió daño. Vida actual: {CurrentHealth}");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Heal(5f);
            Debug.Log($"Jugador curado. Vida actual: {CurrentHealth}");
        }
        Debug.Log($"actual: {CurrentHealth}");
    }
}
