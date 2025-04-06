using TMPro;
using UnityEngine;

public class ComboSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    private AttackBase attack;

    public void Init(AttackBase atk)
    {
        attack = atk;
        nameText.text = attack.attackName;
    }
}
