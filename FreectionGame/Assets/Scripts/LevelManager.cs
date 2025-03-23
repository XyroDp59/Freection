using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public List<LevelData> levels;
    [NonSerialized]
    public SerializedLevelData currentLevelData = null;

    public static LevelManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        currentLevelData = null;
    }

    public void LoadLevel(string levelName)
    {
        if (currentLevelData != null)
        {
            ExitLevel(false);
        }

        Goal.hasFinished = false;

        LevelData newLevel = levels.Find(ld => ld.levelSceneName == levelName && ld.unlocked);
        if (newLevel != null) 
        {
            SerializedLevelData newLevelData = SerializedLevelData.LoadLevelData(levelName);
            currentLevelData = newLevelData;
            SceneManager.LoadScene(levelName);
        }
    }

    public void ResetLevel(PlayerControls player)
    {
        if (Goal.hasFinished)
        {
            TimerManager.instance.RefreshCheckpointTimes();
            GameUI.instance.ShowEndScreen(false);
            PlayerControls.Instance.blockGameInputs = false;
            Goal.hasFinished = false;
        }

        GameUI.instance.ShowPauseMenu(false);

        player.Respawn(CheckpointManager.instance.spawnPoint, false);
    }

    public void ExitLevel(bool toMainMenu)
    {
        if (currentLevelData == null) return;

        Time.timeScale = 1.0f;

        TimerManager.instance.StoreTimes();
        SerializedLevelData.SaveLevelData(currentLevelData.levelSceneName, currentLevelData);

        if (toMainMenu)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void WinLevel()
    {
        Goal.hasFinished = true;
        if (!currentLevelData.isWon) currentLevelData.isWon = true;
        PlayerControls.Instance.blockGameInputs = true;
        TimerManager.instance.StopTimer();
        GameUI.instance.ShowEndScreen(true);
    }
}
