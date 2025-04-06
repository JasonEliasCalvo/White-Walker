using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void DelegatedGameStates();
    public DelegatedGameStates eventGameStart;
    public DelegatedGameStates eventGameEnd;
    public DelegatedGameStates eventHackingMiniGameStart;
    public DelegatedGameStates eventHackingMiniGameReset;
    public DelegatedGameStates eventHackingMiniGameEnd;
    public static GameManager instance;

    [SerializeField] private CombatData combatData;
    private PlayerSaveData currentData;

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


    void Start()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(currentData, true);
        PlayerPrefs.SetString("SaveData", json);
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("SaveData"))
        {
            string json = PlayerPrefs.GetString("SaveData");
            currentData = JsonUtility.FromJson<PlayerSaveData>(json);
            combatData.LoadFromData(currentData); // Esta sí existe si la agregaste arriba
        }
        else
        {
            currentData = new PlayerSaveData(); // Nuevo juego
        }
    }

    public void OnClick_SaveButton()
    {
        SaveGame();
    }

    public void GamePrepate()
    {
        timer = FindAnyObjectByType<Timer>();
        Invoke(nameof(GameStart), 0.2f);
    }

    public void GameStart()
    {
        eventGameStart?.Invoke();
        HackingMiniGameStart();
    }

    public void GamePause()
    {

    }
    public void GameResume()
    {

    }

    public void HackingMiniGameStart()
    {
        Debug.Log("initiateTime: " + initiateTime);
        timer.eventEndTime += ResetHackingMiniGame;
        timer.Initiate(initiateTime);
        eventHackingMiniGameStart?.Invoke();
    }

    public void ResetHackingMiniGame()
    {
        eventHackingMiniGameReset?.Invoke();
    }

    public void HackingMiniGameEnd()
    {
        eventHackingMiniGameEnd?.Invoke();
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
