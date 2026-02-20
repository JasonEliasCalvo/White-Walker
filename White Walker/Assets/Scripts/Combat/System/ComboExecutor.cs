using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ComboEvents
{
    public static Action OnComboChanged;
}

public class ComboExecutor : MonoBehaviour
{
    [Header("Configuración")]
    public StyleType currentWeapon;
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

    [SerializeField] private float gunCooldown = 0.8f;
    private float lastGunAttackTime;

    public GameObject fullBodyGhostPrefab;
    public GameObject weaponOnlyGhostPrefab;
    private Dictionary<StyleType, int> lastComboIndex = new();
    private Dictionary<StyleType, float> lastComboTime = new();

    private void Start()
    {
        LoadCurrentCombo();
        ComboEvents.OnComboChanged += OnComboChanged;
        Debug.Log($"[ComboExecutor] Cambiaste a arma: {currentWeapon}");
    }

    private void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) TryAttack();
        if (Input.GetMouseButtonDown(1)) TryGunAttack();
        if (Input.GetKeyDown(KeyCode.Tab)) SwitchWeapon();
    }

    void SwitchWeapon()
    {

        if (isAttacking && currentIndex > 0 && currentIndex <= currentCombo.Count)
        {
            var atk = currentCombo[currentIndex - 1];
            if (atk.spawnGhostOnSwap)
            {
                SpawnAttackGhost(atk);
            }
            SaveComboProgress();
        }

        currentWeapon = currentWeapon switch
        {
            StyleType.Unarmed => StyleType.Armed,
            StyleType.Armed => StyleType.Unarmed,
            _ => StyleType.Unarmed
        };

        LoadCurrentCombo();
        Debug.Log($"[ComboExecutor] Cambiaste a arma: {currentWeapon}");
    }

    void SpawnAttackGhost(AttackBase atk)
    {
        var ghost = Instantiate(weaponOnlyGhostPrefab, attackPoint.position, attackPoint.rotation);
        var ghostScript = ghost.GetComponent<AttackGhost>();
        ghostScript.Init(atk);
    }

    void SaveComboProgress()
    {
        lastComboIndex[currentWeapon] = currentIndex;
        lastComboTime[currentWeapon] = Time.time;
    }

    void ApplyEffects(Collider hit, AttackBase atk)
    {
        var enemy = hit.GetComponent<EnemyController>();
        if (enemy == null) return;

        foreach (var effect in atk.effects)
        {
            switch (effect.tag)
            {
                case EffectTag.AirLauncher:
                    enemy.Stun(effect.duration);
                    break;
                case EffectTag.Knockdown:
                    enemy.KnockDown(effect.duration);
                    break;
                    // Agrega más efectos aquí si es necesario
            }
        }
    }

    List<AttackBase> GetCombo(StyleType weaponType)
    {
        var ids = GetComboIDs(weaponType);
        return ids.Select(id => CombatManager.Instance.GetAttackById(id))
                  .Where(atk => atk != null)
                  .ToList();
    }

    void LoadCurrentCombo()
    {
        currentCombo = GetCombo(currentWeapon);

        if (lastComboTime.TryGetValue(currentWeapon, out float lastTime) &&
        Time.time - lastTime < maxComboDelay &&
        lastComboIndex.TryGetValue(currentWeapon, out int lastIdx))
        {
            currentIndex = lastIdx;
        }
        else
        {
            currentIndex = 0;
        }
    }

    private void OnComboChanged()
    {
        Debug.Log("[ComboExecutor] Combo actualizado desde ComboEditor");
        LoadCurrentCombo();
        currentIndex = 0;
        gunIndex = 0;
    }

    List<int> GetComboIDs(StyleType weapon)
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
                StyleType.Unarmed => inv.comboClaw,
                StyleType.Armed => inv.comboSword,
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

        AssistRotationToEnemy();

        AttackBase atk = currentCombo[currentIndex];
        ExecuteAttack(atk);

        currentIndex++;
        lastAttackTime = Time.time;
    }

    void TryGunAttack()
    {
        if (Time.time - lastGunAttackTime < gunCooldown) return;

        if (gunIndex >= gunCombo.Count) gunIndex = 0;

        if (gunCombo.Count == 0) return;

        var atk = gunCombo[gunIndex];
        Debug.Log($"[ComboExecutor] Ataque con pistola");

        AssistRotationToEnemy();

        ExecuteAttack(atk);
        gunIndex++;

        lastGunAttackTime = Time.time;
    }

    void AssistRotationToEnemy(float range = 3f, float maxAngle = 60f, float maxRotationDegrees = 20f)
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, range, enemyLayer);

        Transform bestTarget = null;
        float bestScore = float.MaxValue;

        foreach (var col in nearby)
        {
            Vector3 dirToEnemy = (col.transform.position - transform.position).normalized;

            float angle = Vector3.Angle(transform.forward, dirToEnemy);
            if (angle > maxAngle) continue; // fuera del cono de visión

            float distance = Vector3.Distance(transform.position, col.transform.position);

            // Calcular una "puntuación" que prioriza ángulo más que distancia
            float score = angle * 1.5f + distance * 0.5f;

            if (score < bestScore)
            {
                bestScore = score;
                bestTarget = col.transform;
            }
        }

        if (bestTarget != null)
        {
            Vector3 direction = (bestTarget.position - transform.position).normalized;
            Quaternion desiredRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, maxRotationDegrees);
        }
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
            if (target == null) continue;

            target.TakeDamage(atk.damage, atk.hitStun);
            ApplyEffects(hit, atk);
            break;
        }

        // reproducir animación: animator.Play(atk.animation.name);
        StartCoroutine(WaitForAttackEnd(atk.TotalDuration));
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

