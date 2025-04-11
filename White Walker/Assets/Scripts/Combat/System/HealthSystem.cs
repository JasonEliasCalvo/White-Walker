using System;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount);
    void Heal(float amount);
}

public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    // Eventos para UI, partículas, animaciones, sonidos
    public event Action OnDamaged;
    public event Action OnHealed;
    public event Action OnDied;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnDamaged?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealed?.Invoke();
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
        OnDied?.Invoke();
    }

    public void SetMaxHealth(float newMax)
    {
        maxHealth = newMax;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}
