using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class FighterAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private string currentStateName;
    public bool IsActionPlaying { get; private set; }

    public void PlayLocomotionState(string stateName, float crossfadeDuration = 0.1f)
    {
        if (IsActionPlaying)
            return;

        if (currentStateName == stateName)
            return;

        currentStateName = stateName;
        animator.CrossFade(stateName, crossfadeDuration);
    }

    public void PlayAction(string stateName, float crossfadeDuration = 0.05f)
    {
        if (animator == null)
            return;

        if (string.IsNullOrEmpty(stateName))
            return;

        currentStateName = stateName;
        animator.CrossFade(stateName, crossfadeDuration);
    }

    public void SetActionPlaying(bool value)
    {
        IsActionPlaying = value;
    }
}