using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    // Configuración de la simulación
    public const float TICK_RATE = 1.0f / 60.0f; // 60 FPS fijos
    private float _accumulator = 0.0f;
    public int currentTick { get; private set; }

    // Referencias a los luchadores en la simulación
    [Header("Simulation Entities")]
    public List<ActorSimState> actors = new List<ActorSimState>();

    // --- ESTRUCTURA DE DATOS DE SIMULACIÓN ---
    [Serializable]
    public class ActorSimState
    {
        public string actorName;
        public FighterEntity view; // Referencia al objeto en Unity (Visual)

        // Datos de transformación (Punto fijo o flotantes puros)
        public Vector3 position;
        public Quaternion rotation;

        // Estado de Combate
        public short currentActionID;
        public int actionTick; // En qué frame de la animación va
        public float health;

        // Buffer de Input
        public InputState lastInput;
    }

    [Serializable]
    public struct InputState
    {
        public Vector2 moveDir;
        public bool attackPressed;
        public bool dashPressed;
    }

    private void Update()
    {
        // El acumulador asegura que la simulación corra a 60fps constantes
        // sin importar si el juego corre a 30fps o 300fps.
        _accumulator += Time.deltaTime;

        while (_accumulator >= TICK_RATE)
        {
            PerformTick();
            _accumulator -= TICK_RATE;
            currentTick++;
        }

        // Después de la simulación, actualizamos la vista (Interpolación visual)
        UpdateVisuals();
    }

    private void PerformTick()
    {
        foreach (var actor in actors)
        {
            // 1. Recolectar Input (Bufferizado)
            ProcessInput(actor);

            // 2. Resolver Física Manual (Collision detection por esferas)
            ApplyMovement(actor);

            // 3. Resolver Animación y Hitboxes (Basado en Ticks de AttackBase)
            UpdateActionState(actor);
        }

        // 4. Resolver Colisiones entre actores (SphereChain)
        ResolveCollisions();
    }

    private void ProcessInput(ActorSimState actor)
    {
        // Aquí es donde el PlayerFighter le pasa su input al Manager.
        // Si es una IA, el script de IA llena estos datos.
        if (actor.view is PlayerFighter player)
        {
            // Mapeamos el input del script que me pasaste al estado de simulación
            actor.lastInput.moveDir = player.GetMovementInput();
            // etc...
        }
    }

    private void ApplyMovement(ActorSimState actor)
    {
        // En lugar de CharacterController.Move, calculamos la posición final aquí.
        // actor.position += actor.lastInput.moveDir * speed * TICK_RATE;
    }

    private void UpdateActionState(ActorSimState actor)
    {
        actor.actionTick++;
        // Aquí leemos tu AttackBase: 
        // Si actor.actionTick == attackBase.events[i].tick -> Abrir Hitbox.
    }

    private void ResolveCollisions()
    {
        // Aquí va tu lógica de SphereChain. 
        // Comprobar distancias entre esferas de Actor A y Actor B.
    }

    private void UpdateVisuals()
    {
        // Sincronizamos los Transforms de Unity con la simulación matemática.
        foreach (var actor in actors)
        {
            actor.view.transform.position = actor.position;
            actor.view.transform.rotation = actor.rotation;
        }
    }
}
