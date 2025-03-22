using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public List<LevelData> levels;
    public LevelData currentLevel;
    bool hasWon = false;

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
        LoadLevel(levels[0].levelSceneName);
    }

    public void LoadLevel(string levelName)
    {
        if (currentLevel != null)
        {
            ExitLevel(false);
        }

        LevelData newLevel = levels.Find(ld => ld.levelSceneName == levelName && ld.unlocked);
        if (newLevel != null) 
        {
            currentLevel = newLevel;
            SceneManager.LoadScene(levelName);
        }
    }

    public void ResetLevel(PlayerControls player)
    {
        if (hasWon)
        {
            TimerManager.instance.RefreshCheckpointTimes();
        }
        player.Respawn(CheckpointManager.instance.spawnPoint, false);
        hasWon = false;
    }

    public void ExitLevel(bool toMainMenu)
    {
        if (currentLevel == null) return;

        TimerManager.instance.StoreTimes(currentLevel.levelSceneName);

        if (toMainMenu)
        {
            //TODO : go to main menu
        }
    }

    public void WinLevel()
    {
        hasWon = true;
        currentLevel.isWon = true;
    }
}
