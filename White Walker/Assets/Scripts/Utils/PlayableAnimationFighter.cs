using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayableAnimationFighter : MonoBehaviour
{
    private PlayableGraph graph;
    private AnimationLayerMixerPlayable layerMixer;

    // Slots: 0 = Locomoción (Animator Controller), 1 = Ataque A, 2 = Ataque B
    private AnimatorControllerPlayable locomotionPlayable;
    private AnimationClipPlayable[] attackSlots = new AnimationClipPlayable[2];

    private int activeSlotIndex = -1; // -1 significa que estamos en locomotion
    private float transitionSpeed;
    private float[] targetWeights = new float[3];
    private float[] currentWeights = new float[3];

    private float hitStopTimer;
    private Vector3 rootMotionDelta;

    void Awake()
    {
        Animator anim = GetComponent<Animator>();
        graph = PlayableGraph.Create($"{gameObject.name}_CombatGraph");

        // Creamos un mixer de 3 entradas
        layerMixer = AnimationLayerMixerPlayable.Create(graph, 3);

        // Input 0: El Animator Controller original (Idle/Walk/Run)
        locomotionPlayable = AnimatorControllerPlayable.Create(graph, anim.runtimeAnimatorController);
        graph.Connect(locomotionPlayable, 0, layerMixer, 0);

        var output = AnimationPlayableOutput.Create(graph, "Animation", anim);
        output.SetSourcePlayable(layerMixer);

        // Estado inicial: Todo a Locomoción
        currentWeights[0] = 1f;
        targetWeights[0] = 1f;
        layerMixer.SetInputWeight(0, 1f);

        graph.Play();
    }

    public void PlayClip(AnimationClip clip, float duration)
    {
        transitionSpeed = 1f / duration;

        // Alternamos entre slot 1 y 2 para el Crossfade (Ping-Pong)
        int nextSlot = (activeSlotIndex == 1) ? 2 : 1;
        int oldSlot = (activeSlotIndex == 1) ? 1 : 2;

        // Limpiar el slot que vamos a usar
        if (attackSlots[nextSlot - 1].IsValid())
        {
            graph.Disconnect(layerMixer, nextSlot);
            attackSlots[nextSlot - 1].Destroy();
        }

        // Crear y conectar el nuevo ataque
        attackSlots[nextSlot - 1] = AnimationClipPlayable.Create(graph, clip);
        graph.Connect(attackSlots[nextSlot - 1], 0, layerMixer, nextSlot);
        attackSlots[nextSlot - 1].SetTime(0);

        // Definir pesos objetivo
        targetWeights[0] = 0f;       // Apagar Locomoción
        targetWeights[nextSlot] = 1f; // Encender nuevo ataque
        targetWeights[oldSlot] = 0f;  // Apagar ataque anterior

        activeSlotIndex = nextSlot;
    }

    public void StopAttack(float duration)
    {
        transitionSpeed = 1f / duration;
        targetWeights[0] = 1f; // Volver a Locomoción
        targetWeights[1] = 0f;
        targetWeights[2] = 0f;
        activeSlotIndex = -1;
    }

    void Update()
    {
        // Suavizado manual de pesos (El Crossfade real)
        for (int i = 0; i < 3; i++)
        {
            currentWeights[i] = Mathf.MoveTowards(currentWeights[i], targetWeights[i], transitionSpeed * Time.deltaTime);
            layerMixer.SetInputWeight(i, currentWeights[i]);
        }
    }

    public double GetAttackNormalizedTime()
    {
        if (activeSlotIndex == -1) return 0;
        var playable = attackSlots[activeSlotIndex - 1];
        if (!playable.IsValid()) return 0;
        return playable.GetTime() / playable.GetAnimationClip().length;
    }

    void OnDestroy() { if (graph.IsValid()) graph.Destroy(); }
}