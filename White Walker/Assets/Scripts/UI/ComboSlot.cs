using TMPro;
using UnityEngine;

public class ComboSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    private AttackData attack;

    public void Init(AttackData atk)
    {
        attack = atk;
        nameText.text = attack.attackName;
    }
}
