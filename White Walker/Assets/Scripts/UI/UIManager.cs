using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Dialogue Elements")]
    [SerializeField] private GameObject interactablePanel;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject HakingPanel;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject ComboEditorPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

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
        //ShowDialoguePanel(false);
    }

    public void Update()
    {
        if (Input.GetKeyUp(dialogueKey))
        {
            ShowComboEditorPanel(true);
        }
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

    public void ShowComboEditorPanel(bool state)
    {
        ComboEditorPanel.SetActive(state);
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel.activeSelf || HakingPanel.activeSelf;
    }
    public TextMeshProUGUI GetDialogueText()
    {
        return dialogueText;
    }
}
