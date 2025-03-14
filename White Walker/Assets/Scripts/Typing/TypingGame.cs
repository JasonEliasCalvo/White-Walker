using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypingGame : TypingController
{
    [SerializeField] private Transform player;
    [SerializeField] private float speed;
    [SerializeField] private Transform startPlataformPosition;
    [SerializeField] private GameObject plataform;
    [SerializeField] private List<Transform> platforms;
    [SerializeField] private float distanceBetweenPlatforms;
    [SerializeField] private Vector3 generationDirection = Vector3.right;
    private Vector3 startPlayerPosition;
    private Vector3 newPlayerPosition;
    private Quaternion newPlayerRotation;

    void Start()
    {
        GameManager.instance.eventTypingGameStart += StartTypingGame;
        GameManager.instance.eventTypingGameEnd += EndTypingGame;
        GameManager.instance.eventTypingGameReset += ResetTypingGame;
    }

    private void StartTypingGame()
    {
        IsTypingGameActive = true;
        GameManager.instance.GameEnd();
        startPlayerPosition = player.position;
        WordText.text = TargetWord;
        CreatePlatforms();
        UIManager.instance.ShowTypingPanel(true);
        UIManager.instance.ShowTimerPanel(true);
        ResetTypingGame();
    }

    void CreatePlatforms()
    {
        foreach (Transform platform in platforms)
        {
            Destroy(platform.gameObject);
        }
        platforms.Clear();

        Vector3 currentPosition = startPlataformPosition.position;

        for (int i = 0; i < TargetWord.Length; i++)
        {
            GameObject newPlatform = Instantiate(plataform, currentPosition, Quaternion.identity);
            platforms.Add(newPlatform.transform);
            newPlatform.SetActive(false);
            currentPosition += generationDirection.normalized * distanceBetweenPlatforms;
        }
    }

    protected override void OnTextSubmitted()
    {
        if (CurrentLetter < TargetWord.Length)
        {
            if (Input.anyKeyDown)
            {
                char pressedKey = GetPressedKey();

                if (pressedKey != '\0')
                {
                    if (pressedKey == TargetWord[CurrentLetter])
                    {
                        platforms[CurrentLetter].gameObject.SetActive(true);

                        newPlayerPosition = new Vector3(platforms[CurrentLetter].position.x, platforms[CurrentLetter].position.y, platforms[CurrentLetter].position.z);
                        newPlayerRotation = Quaternion.LookRotation(generationDirection);

                        CurrentLetter++;

                        if (CurrentLetter == TargetWord.Length)
                        {
                            MessageText.text = "¡Has completado la palabra!";
                            GameManager.instance.GetTimer().Stop();
                            EndTypingGame();
                        }
                    }
                    else
                    {
                        MessageText.text = "¡Error! Comienza de nuevo.";
                        ResetTypingGame();
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsTypingGameActive)
        {
            player.position = Vector3.Lerp(player.position, newPlayerPosition, speed);
            player.rotation = Quaternion.Slerp(player.rotation, newPlayerRotation,speed);
        }
        
    }
   
    private void EndTypingGame()
    {
        IsTypingGameActive = false;
        StartCoroutine(EndTyping());
    }

    void ResetTypingGame()
    {
        CurrentLetter = 0;
        newPlayerPosition = startPlayerPosition;
        player.position = startPlayerPosition;
        GameManager.instance.GetTimer().Reboot();

        foreach (Transform platform in platforms)
        {
            platform.gameObject.SetActive(false);
        }
    }

    IEnumerator EndTyping()
    {
        yield return new WaitForSeconds(1f);
        GameManager.instance.GameStart();
        UIManager.instance.ShowTypingPanel(false);
        UIManager.instance.ShowTimerPanel(false);
    }
}
