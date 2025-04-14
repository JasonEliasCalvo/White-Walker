using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum InteractionType
{
    StartDialogue,
    StartMoving,
    StopMoving,
    StartHackingMiniGame
}

public class InteractableOptions : MonoBehaviour
{
    
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private int ID;
    [SerializeField] private bool possibleInteract = true;
    private bool isPlayerInTrigger = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (possibleInteract)
        {
            if (Input.GetKeyDown(UIManager.instance.dialogueKey) && isPlayerInTrigger)
            {
                if (!UIManager.instance.IsPanelActive())
                {
                    ExecuteInteraction();
                    UIManager.instance.ShowInteractablePanel(false);
                    PlayerOutTrigger();
                }
            }
        }
    }

    private void ExecuteInteraction()
    {
        switch (interactionType)
        {
            case InteractionType.StartDialogue:
                break;
            
            case InteractionType.StartMoving:
                break;

            case InteractionType.StartHackingMiniGame:
                GameManager.instance.HackingMiniGameStart();
                    break;
        }
    }

    public void PlayerInTrigger() => isPlayerInTrigger = true; 
    public void PlayerOutTrigger() => isPlayerInTrigger = false; 
    public void StartInterract() => possibleInteract = true;
    public void StopInterract() => possibleInteract = false;
}
