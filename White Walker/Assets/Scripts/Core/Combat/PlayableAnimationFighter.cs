using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayableAnimationFighter : MonoBehaviour
{
    private PlayableGraph graph;
    private AnimationLayerMixerPlayable mixer;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // 1. Creamos el Grafo
        graph = PlayableGraph.Create($"{gameObject.name}_CombatGraph");
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        // 2. Creamos un Mixer para poder hacer Crossfade (mezcla)
        mixer = AnimationLayerMixerPlayable.Create(graph, 1);

        // 3. Conectamos el Mixer a la salida del Animator
        var output = AnimationPlayableOutput.Create(graph, "Animation", animator);
        output.SetSourcePlayable(mixer);

        graph.Play();
    }

    public void PlayClip(AnimationClip clip, float transitionDuration)
    {
        // 4. Creamos un "Playable" a partir del clip
        var clipPlayable = AnimationClipPlayable.Create(graph, clip);

        // 5. Lo conectamos al mixer en el slot 0
        // Para un sistema pro, usarías dos slots y harías un lerp de pesos (Weight)
        // Pero para empezar, Play() directo es más limpio:
        mixer.DisconnectInput(0);
        mixer.ConnectInput(0, clipPlayable, 0);
        mixer.SetInputWeight(0, 1.0f);

        clipPlayable.SetTime(0);
    }

    void OnDestroy() => graph.Destroy();
}
