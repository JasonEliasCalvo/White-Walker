using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Dialogue Elements")]
    [SerializeField] private GameObject interactablePanel;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI choicesText;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("UI Game Elements")]
    [SerializeField] private GameObject typingPanel;
    [SerializeField] private GameObject timerPanel;

    [Header("UI Settings")]
    public KeyCode dialogueKey = KeyCode.F;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Start()
    {
        instance.ShowDialoguePanel(false);
        instance.ShowChoicesPanel(false);
    }

    public void ShowInteractablePanel(bool state)
    {
        if (!IsDialogueActive())
        {
            interactablePanel.SetActive(state);
        }
        else
        {
            interactablePanel.SetActive(false);
        }
    }

    public void ShowDialoguePanel(bool state)
    {
        dialoguePanel.SetActive(state);
    }

    public void ShowChoicesPanel(bool state)
    {
        choicesPanel.SetActive(state);
    }

    public void ShowTimerPanel(bool state)
    {
        timerPanel.SetActive(state);
    }

    public GameObject GetChoiceButtonPrefab()
    {
        return choiceButtonPrefab;
    }

    public Transform GetChoiceContainer()
    {
        return choicesContainer;
    }

    public void ShowTypingPanel(bool state)
    {
        typingPanel.SetActive(state);
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel.activeSelf || choicesPanel.activeSelf;
    }
    public TextMeshProUGUI GetDialogueText()
    {
        return dialogueText;
    }

    public TextMeshProUGUI GetQuestionText()
    {
        return choicesText;
    }
}
