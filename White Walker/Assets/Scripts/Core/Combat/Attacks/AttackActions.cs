using UnityEngine;
using System;

namespace Combat
{
    // --- TIPOS DE CANCELACIÓN ---
    public enum CancelType { None, Attack, Dodge, GuardBreak, Any }

    [Serializable]
    public class CancelWindow
    {
        [Tooltip("Frame donde inicia la ventana para cancelar")]
        public int startFrame;
        [Tooltip("Frame donde termina la ventana")]
        public int endFrame;
        public CancelType cancelType;
    }

    // --- CLASE BASE PARA ACCIONES ---
    [Serializable]
    public class AttackAction
    {
        public int startFrame;
        public int endFrame;

        protected bool hasStarted = false;

        public void Tick(FighterEntity fighter, int currentFrame)
        {
            // START
            if (!hasStarted && currentFrame >= startFrame)
            {
                hasStarted = true;
                OnStart(fighter);
            }

            // ACTIVE
            if (currentFrame >= startFrame && currentFrame <= endFrame)
            {
                OnActive(fighter, currentFrame);
            }

            // END
            if (hasStarted && currentFrame > endFrame)
            {
                OnEnd(fighter);
                hasStarted = false; // reset por si se reutiliza
            }
        }

        protected virtual void OnStart(FighterEntity fighter) { }
        protected virtual void OnActive(FighterEntity fighter, int currentFrame) { }
        protected virtual void OnEnd(FighterEntity fighter) { }
    }

    // --- ACCIÓN 1: CONTROL DE HITBOX ---
    [Serializable]
    public class HitboxAction : AttackAction
    {
        public int hitboxIndex;

        public float damage = 10f;
        public float hitStun = 0.5f;
        public float knockbackForce = 5f;

        protected override void OnStart(FighterEntity fighter)
        {
            fighter.OpenHitbox(hitboxIndex, damage, hitStun, knockbackForce);
        }

        protected override void OnEnd(FighterEntity fighter)
        {
            fighter.CloseHitbox(hitboxIndex);
        }
    }

    // --- ACCIÓN 2: IMPULSO DE MOVIMIENTO (Ej: God Hand Step-in punch) ---
    [Serializable]
    public class MovementAction : AttackAction
    {
        public Vector3 localDirection = Vector3.forward;
        public float speed;

        protected override void OnActive(FighterEntity fighter, int currentFrame)
        {
            Vector3 worldDir = fighter.transform.TransformDirection(localDirection);
            fighter.MoveEntity(worldDir, speed);
        }
    }
}