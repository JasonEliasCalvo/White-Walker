using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComboEvents
{
    public static Action OnComboChanged;
}

public class ComboExecutor : MonoBehaviour
{
    [Header("Configuración")]
    public WeaponType currentWeapon;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask enemyLayer;
    public float maxComboDelay = 1.2f;

    private List<AttackBase> currentCombo = new();
    private List<AttackBase> gunCombo = new();
    private int currentIndex = 0;
    private int gunIndex = 0;
    private float lastAttackTime;
    private bool isAttacking = false;

    private void Start()
    {
        LoadCurrentCombo();
        LoadGunCombo();
        ComboEvents.OnComboChanged += OnComboChanged;
        Debug.Log($"[ComboExecutor] Cambiaste a arma: {currentWeapon}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryAttack();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            TryGunAttack();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchWeapon();
        }
    }

    void SwitchWeapon()
    {
        currentWeapon = currentWeapon switch
        {
            WeaponType.Claw => WeaponType.Sword,
            WeaponType.Sword => WeaponType.Claw,
            _ => WeaponType.Claw
        };

        LoadCurrentCombo();
        currentIndex = 0;
        Debug.Log($"[ComboExecutor] Cambiaste a arma: {currentWeapon}");
    }

    void LoadCurrentCombo()
    {
        currentCombo.Clear();
        List<int> comboIDs = GetComboIDs(currentWeapon);

        foreach (int id in comboIDs)
        {
            var atk = CombatManager.Instance.GetAttackById(id);
            if (atk != null)
                currentCombo.Add(atk);
        }
    }

    void LoadGunCombo()
    {
        gunCombo.Clear();
        List<int> comboIDs = GetComboIDs(WeaponType.Gun);

        foreach (int id in comboIDs)
        {
            var atk = CombatManager.Instance.GetAttackById(id);
            if (atk != null)
                gunCombo.Add(atk);
        }
    }

    private void OnComboChanged()
    {
        Debug.Log("[ComboExecutor] Combo actualizado desde ComboEditor");
        LoadCurrentCombo();
        LoadGunCombo();
        currentIndex = 0;
        gunIndex = 0;
    }

    List<int> GetComboIDs(WeaponType weapon)
    {
        if (PlayerSaveManager.Instance.currentMode == SaveMode.Json)
        {
            var combo = PlayerSaveManager.Instance.CurrentSave.comboData
                .equippedCombos.Find(c => c.weaponType == weapon);
            return combo?.attackIDs ?? new List<int>();
        }
        else
        {
            var inv = PlayerSaveManager.Instance.inventorySO;
            return weapon switch
            {
                WeaponType.Claw => inv.comboClaw,
                WeaponType.Sword => inv.comboSword,
                WeaponType.Gun => inv.comboGun,
                _ => new List<int>()
            };
        }
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime > maxComboDelay)
        {
            currentIndex = 0;
        }

        if (currentIndex >= currentCombo.Count) return;

        AttackBase atk = currentCombo[currentIndex];
        ExecuteAttack(atk);

        currentIndex++;
        lastAttackTime = Time.time;
    }

    void TryGunAttack()
    {
        if (gunIndex >= gunCombo.Count) gunIndex = 0;

        if (gunCombo.Count == 0) return;

        var atk = gunCombo[gunIndex];
        Debug.Log($"[ComboExecutor] Ataque con pistola");
        ExecuteAttack(atk);
        gunIndex++;
    }

    void ExecuteAttack(AttackBase atk)
    {
        isAttacking = true;
        Debug.Log($"[ComboExecutor] Ejecutando ataque: {atk.attackName} ({atk.category})");

        // Detectar enemigos
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayer);
        foreach (var hit in hits)
        {
            var target = hit.GetComponent<IDamageable>();

            if (target != null)
            {
                target.TakeDamage(atk.damage);              
                Debug.Log("Golpeaste a: " + hit.name);

                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    if (AttackUtils.HasEffect(atk, EffectTag.Stun))
                        enemy.Stun(AttackUtils.GetEffectDuration(atk, EffectTag.Stun));

                    if (AttackUtils.HasEffect(atk, EffectTag.Knockdown))
                        enemy.KnockDown(AttackUtils.GetEffectDuration(atk, EffectTag.Knockdown));
                }
                break;
            }
        }

        // reproducir animación: animator.Play(atk.animation.name);
        StartCoroutine(WaitForAttackEnd(atk.duration));
    }

    IEnumerator WaitForAttackEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }

    private void OnDestroy()
    {
        ComboEvents.OnComboChanged -= OnComboChanged;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}

