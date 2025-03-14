using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void DelegatedGameStates();
    public DelegatedGameStates eventGameStart;
    public DelegatedGameStates eventGameEnd;
    public DelegatedGameStates eventTypingGameStart;
    public DelegatedGameStates eventTypingGameReset;
    public DelegatedGameStates eventTypingGameEnd;
    public static GameManager instance;

    private Timer timer;
    [SerializeField] float initiateTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GamePrepate();
    }

    public void GamePrepate()
    {
        timer = FindAnyObjectByType<Timer>();
        Invoke(nameof(GameStart), 0.2f);
    }

    public void GameStart()
    {
        eventGameStart?.Invoke();
        TypingGameStart();
    }

    public void GamePause()
    {

    }
    public void GameResume()
    {

    }

    public void TypingGameStart()
    {
        Debug.Log("initiateTime: " + initiateTime);
        timer.eventEndTime += ResetTypingGame;
        timer.Initiate(initiateTime);
        eventTypingGameStart?.Invoke();
    }

    public void ResetTypingGame()
    {
        eventTypingGameReset?.Invoke();
    }

    public void TypingGameEnd()
    {
        eventTypingGameEnd?.Invoke();
    }

    public void GameEnd()
    {
        eventGameEnd?.Invoke();
    }

    public Timer GetTimer()
    {
        return timer;
    }
}
