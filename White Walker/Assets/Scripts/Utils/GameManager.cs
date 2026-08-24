using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Minijuego")]
    [SerializeField] float initiateTime;

    public delegate void DelegatedGameStates();
    public DelegatedGameStates eventGameStart;
    public DelegatedGameStates eventGameEnd;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        GamePrepare();
    }

    private void GamePrepare()
    {
        Invoke(nameof(GameStart), 0.2f);
    }

    public void GameStart() => eventGameStart?.Invoke();
    public void GamePause() { }
    public void GameResume() { }

    public void GameEnd() => eventGameEnd?.Invoke();

}
