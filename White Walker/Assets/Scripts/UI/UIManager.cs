using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Dialogue Elements")]
    [SerializeField] private GameObject interactablePanel;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject HackingPanel;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject ComboEditorPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;


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
    }

    public void ShowInteractablePanel(bool state)
    {
        if (!IsPanelActive())
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

    public bool IsPanelActive()
    {
        return dialoguePanel.activeSelf || HackingPanel.activeSelf || ShopPanel.activeSelf || ComboEditorPanel.activeSelf;
    }

    public TextMeshProUGUI GetDialogueText()
    {
        return dialogueText;
    }
}
