using UnityEngine;
using static FighterEntity;

public class EnemyFighter : FighterEntity, IFighterInput
{
    // Lógica simple de IA (Dummy)
    public bool AttackPressed()
    {
        // ejemplo simple
        return Random.value < 0.01f;
    }

    public bool DodgePressed()
    {
        return false;
    }

    public Vector2 MoveInput()
    {
        return Vector2.zero;
    }

    public void ConsumeInput() { }

    public override Vector3 GetMovementInput()
    {
        // Por ahora el dummy no se mueve
        return Vector3.zero;
    }

    public override bool GetAttackInput()
    {
        // Por ahora el dummy no ataca, solo recibe golpes
        return false;
    }

    // Si tuvieras que añadir lógica específica (como mirar al jugador), iría en Update
    protected override void Update()
    {
        base.Update(); // Importante mantener el update del padre
    }
}