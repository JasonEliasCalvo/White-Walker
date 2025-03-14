using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class StandartButton : MonoBehaviour
{
    public TextMeshProUGUI answer;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void SetAnswer(string answerText)
    {
        answer.text = answerText;
    }

    public void SetButtonAction(UnityEngine.Events.UnityAction action)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
