using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class DialogueSystem : MonoBehaviour
{
    [Serializable]
    public class DialoguePreData
    {
        public int indexText;
        public DialogueData preDialogueData;
        [Space]
        public UnityEvent onDialogueFinished;

    }

    [Serializable]
    public class QuestionPreData
    {
        public int indexText;
        public QuestionData preQuestionData;
        [Space]
        public UnityEvent onChoiceCorrectEnd;
        public UnityEvent onChoiceIncorrectEnd;
    }

    private DialoguePreData currentDialogue;
    private QuestionPreData currentQuestion;

    public List<DialoguePreData> preDialoguesDatas = new List<DialoguePreData>();
    public List<QuestionPreData> preQuestionsDatas = new List<QuestionPreData>();

    private int currentLine;
    public float typingSpeed = 0.05f;
    public bool isChoiceEnd = false;

    private List<GameObject> choiceButtons = new List<GameObject>();

    public void StartDialogue(int indexDialogueData)
    {
        int _indexTemp = 0;
        foreach (DialoguePreData value in preDialoguesDatas)
        {
            if (value.indexText != indexDialogueData)
            {
                _indexTemp++;
            }
            else
            {
                break;
            }
        }

        currentDialogue = preDialoguesDatas[_indexTemp];
        currentLine = 0;
        UIManager.instance.ShowDialoguePanel(true);
        UIManager.instance.ShowChoicesPanel(false);
        StartCoroutine(ShowDialogueLine());
        GameManager.instance.GameEnd();
    }

    private void Update()
    {
        if (currentDialogue == null) return;

        if (isChoiceEnd && Input.GetKeyDown(UIManager.instance.dialogueKey)) 
        {
            EndDialogue();
        } 
        else if (Input.GetKeyDown(UIManager.instance.dialogueKey) && currentLine < currentDialogue.preDialogueData.dialogueLines.Count)
        {
            if (UIManager.instance.GetDialogueText().text == currentDialogue.preDialogueData.dialogueLines[currentLine])
            {
                NextDialogueLine();
            }
            else
            {
                StopAllCoroutines();
                UIManager.instance.GetDialogueText().text = currentDialogue.preDialogueData.dialogueLines[currentLine];
            }
        }
    }

    IEnumerator ShowDialogueLine()
    {
        UIManager.instance.GetDialogueText().text = string.Empty;

        foreach (char ch in currentDialogue.preDialogueData.dialogueLines[currentLine])
        {
            UIManager.instance.GetDialogueText().text += ch;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void NextDialogueLine()
    {
        currentLine++;
        if (currentLine < currentDialogue.preDialogueData.dialogueLines.Count)
        {
            StartCoroutine(ShowDialogueLine());
        }
        else
        {
            EndDialogue();
            currentDialogue.onDialogueFinished?.Invoke();
        }
    }

    public void ShowChoices(int indexDataQuestion)
    {
        int _indexTemp = 0;
        foreach (QuestionPreData value in preQuestionsDatas)
        {
            if (value.indexText != indexDataQuestion)
            {
                _indexTemp++;
            }
            else
            {
                break;
            }
        }

        GameManager.instance.GameEnd();
        currentQuestion = preQuestionsDatas[indexDataQuestion];
        UIManager.instance.ShowChoicesPanel(true);
        UIManager.instance.ShowDialoguePanel(false);
        UIManager.instance.GetQuestionText().text = currentQuestion.preQuestionData.questionText;

        ClearChoices();

        for (int i = 0; i < currentQuestion.preQuestionData.options.Length; i++)
        {
            GameObject _buttonTemp = Instantiate(UIManager.instance.GetChoiceButtonPrefab(), UIManager.instance.GetChoiceContainer());
            if (_buttonTemp != null)
            {
                int choiceIndex = i;
                _buttonTemp.GetComponent<StandartButton>().SetAnswer(currentQuestion.preQuestionData.options[i]);
                _buttonTemp.GetComponent<StandartButton>().SetButtonAction(() => SelectChoice(choiceIndex));
            }
            choiceButtons.Add(_buttonTemp);
        }
    }

    private void ClearChoices()
    {
        foreach (GameObject button in choiceButtons)
        {
            Destroy(button);
        }
        choiceButtons.Clear();
    }

    public void SelectChoice(int choiceIndex)
    {
        if (choiceIndex == currentQuestion.preQuestionData.correctAnswerIndex)
        {
            CorrectChoice();
        }
        else
        {
            IncorrectChoice();
        }
    }

    private void CorrectChoice()
    {
        UIManager.instance.ShowDialoguePanel(true);
        UIManager.instance.ShowChoicesPanel(false);
        UIManager.instance.GetDialogueText().text = currentQuestion.preQuestionData.correctFeedback;
        currentQuestion.onChoiceCorrectEnd?.Invoke();
        isChoiceEnd = true;
    }

    private void IncorrectChoice()
    {
        UIManager.instance.ShowDialoguePanel(true);
        UIManager.instance.ShowChoicesPanel(false);
        UIManager.instance.GetDialogueText().text = currentQuestion.preQuestionData.incorrectFeedback;
        currentQuestion.onChoiceIncorrectEnd?.Invoke();
        isChoiceEnd = true;
    }

    public void EndDialogue()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);
        currentLine = 0;
        UIManager.instance.GetDialogueText().text = string.Empty;
        UIManager.instance.ShowDialoguePanel(false);
        GameManager.instance.GameStart();
        isChoiceEnd = false;
    }


}
