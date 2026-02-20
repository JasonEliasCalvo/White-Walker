using UnityEngine;

public class CombatComponent : MonoBehaviour
{
    [SerializeField] private AttackBase[] attackList;

    private FighterEntity owner;
    private Animator animator;

    private int currentAttackIndex = -1;
    private float stateTimer;

    public bool IsAttacking { get; private set; }

    private void Awake()
    {
        owner = GetComponent<FighterEntity>();
        animator = GetComponent<Animator>();
    }

    public void StartAttack(int attackIndex, FighterEntity target = null)
    {
        if (IsAttacking) return;
        if (attackIndex < 0 || attackIndex >= attackList.Length) return;

        AttackBase attack = attackList[attackIndex];

        if (!attack.CanExecute(owner, target))
            return;

        currentAttackIndex = attackIndex;
        stateTimer = 0f;
        IsAttacking = true;

        owner.currentAttack = attackList[currentAttackIndex];

        animator.CrossFade(attack.animation.name, 0.05f);
    }

    private void Update()
    {
        if (!IsAttacking) return;

        stateTimer += Time.deltaTime;

        AttackBase attack = attackList[currentAttackIndex];
        HandleFrameData(attack);
    }

    private void HandleFrameData(AttackBase attack)
    {
        float totalDuration = attack.startupTime + attack.activeTime + attack.recoveryTime;

        if (stateTimer >= totalDuration)
        {
            EndAttack();
            return;
        }

        // Hitbox se controla por Animation Events.
        // Aquí solo controlamos finalización.
        switch (attack.executionType)
        {
            case ExecutionType.Grab:
                // Aquí podrías validar agarre si quieres lógica temporal
                break;

            case ExecutionType.Finisher:
                // Aquí podrías bloquear cancelaciones
                break;
        }
    }

    private void EndAttack()
    {
        IsAttacking = false;
        currentAttackIndex = -1;
        owner.currentAttack = null;
    }
}