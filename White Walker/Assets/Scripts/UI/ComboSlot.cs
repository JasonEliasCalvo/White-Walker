using TMPro;
using UnityEngine;

public class ComboSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    private ActionData attack;

    public void Init(ActionData atk)
    {
        attack = atk;
        nameText.text = attack.actionName;
    }
}
