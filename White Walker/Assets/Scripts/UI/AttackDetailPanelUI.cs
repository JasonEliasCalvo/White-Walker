using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackDetailPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI attackNameText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI effectsText;
    [SerializeField] private Button buyButton;

    private AttackBase currentData;
    private ShopManager parentPanel;
    private bool isUnlocked;

    public void ShowAttack(AttackBase attack, bool unlocked, bool owned, ShopManager parent)
    {
        currentData = attack;
        isUnlocked = unlocked;
        parentPanel = parent;

        attackNameText.text = isUnlocked ? attack.attackName : "???";
        damageText.text = isUnlocked ? attack.damage.ToString() : "---";
        costText.text = isUnlocked ? attack.cost.ToString() : "---";
        effectsText.text = isUnlocked
            ? string.Join(", ", attack.effects.Select(e => e.tag.ToString()))
            : "---";

        buyButton.interactable = isUnlocked && !owned;
        buyButton.gameObject.SetActive(isUnlocked && !owned);
        buyButton.onClick.AddListener(() => OnBuyButtonClicked());
        gameObject.SetActive(true);
    }

    private void OnBuyButtonClicked()
    {
        if (currentData != null && isUnlocked)
        {
            parentPanel.TryBuyAttack(currentData);
        }
    }           

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
