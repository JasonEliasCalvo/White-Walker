using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI attackNameText;
    private CanvasGroup canvasGroup;
    private Button button;

    private AttackData currentAttack;
    private bool isUnlocked;
    private bool isOwned;
    private Action<AttackData> onSelect;


    public void Awake()
    {
        button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void Setup(AttackData attack, bool unlocked, bool owned, Action<AttackData> onSelectCallback)
    {
        currentAttack = attack;
        isUnlocked = unlocked;
        isOwned = owned;
        onSelect = onSelectCallback;

        UpdateVisual();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onSelect?.Invoke(currentAttack));
    }

    private void UpdateVisual()
    {
        if (!isUnlocked)
        {
            attackNameText.text = "??? - ???G";
            button.interactable = false;
            canvasGroup.alpha = 0.3f;
        }
        else if (isOwned)
        {
            attackNameText.text = $"{currentAttack.attackName} - Comprado";
            button.interactable = false;
            canvasGroup.alpha = 0.5f;
        }
        else
        {
            attackNameText.text = $"{currentAttack.attackName} - {currentAttack.cost}G";
            button.interactable = true;
            canvasGroup.alpha = 1f;
        }
    }

    public bool IsUnlocked() => isUnlocked;
}
