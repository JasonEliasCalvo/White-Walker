using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI effectsText;
    [SerializeField] private Button buyButton;

    public void Setup(AttackBase attack, bool unlocked, System.Action onClick)
    {
        if (unlocked)
        {
            nameText.text = attack.attackName;
            damageText.text = $"DMG: {attack.damage}";
            costText.text = "Buy: 100"; // Puede cambiar
            effectsText.text = string.Join(", ", attack.effects.ConvertAll(e => e.tag.ToString()));
        }
        else
        {
            nameText.text = "???";
            damageText.text = "DMG: ???";
            costText.text = "Buy: ???";
            effectsText.text = "???";
        }

        buyButton.onClick.RemoveAllListeners();
        buyButton.interactable = unlocked;
        buyButton.onClick.AddListener(() => onClick?.Invoke());
    }
}
