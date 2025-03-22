using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class CheckpointTime
{
    public int checkpointOrder;
    public float time;
}

public class TimerManager : MonoBehaviour
{
    private float gameTime;
    private bool isTimeStarted = false;
    private bool isTimePaused = false;
    private bool isTimeStopped = false;

    float bestTime = float.MaxValue;
    List<CheckpointTime> checkpointTimes = new List<CheckpointTime>();
    List<CheckpointTime> previousCheckpointTimes = new List<CheckpointTime>();

    [SerializeField] GameObject gameTimer;
    private TextMeshProUGUI gameTimerText;
    [SerializeField] GameObject checkpointTimer;
    private TextMeshProUGUI checkpointTimerText;
    [SerializeField] GameObject checkpointDelta;
    private TextMeshProUGUI checkpointDeltaText;

    Coroutine showCheckpoint;

    public static TimerManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadTimes();

        gameTimerText = gameTimer.GetComponentInChildren<TextMeshProUGUI>();
        checkpointTimerText = checkpointTimer.GetComponentInChildren<TextMeshProUGUI>();
        checkpointDeltaText = checkpointDelta.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimeStarted && !isTimePaused && !isTimeStopped)
        {
            gameTime += Time.deltaTime;

            gameTimerText.text = TimeToString(gameTime);
        }
    }

    public void StartTimer()
    {
        isTimeStarted = true;
        isTimeStopped = false;
        gameTime = 0.0f;
    }

    public void PauseTimer(bool isPaused)
    {
        isTimePaused = isPaused;
    }

    public void StopTimer()
    {
        isTimeStopped = true;
    }

    public void ResetTimer()
    {
        StartTimer();
        gameTimerText.text = TimeToString(gameTime);

        if (!Goal.hasFinished && !LevelManager.instance.currentLevelData.isWon)
        {
            previousCheckpointTimes = checkpointTimes;
        }
        checkpointTimes.Clear();

        InterruptShowCheckpoint();

    }

    public void ReachCheckpoint(Checkpoint checkpoint)
    {
        CheckpointTime newTime = new CheckpointTime();
        newTime.checkpointOrder = checkpoint.order;
        newTime.time = gameTime;
        checkpointTimes.Add(newTime);

        InterruptShowCheckpoint();
        showCheckpoint = StartCoroutine(ShowCheckpointTime(newTime));
    }

    private IEnumerator ShowCheckpointTime(CheckpointTime checkpointTime)
    {
        CheckpointTime previousTime = previousCheckpointTimes.Find(ct => ct.checkpointOrder == checkpointTime.checkpointOrder);
        bool showDelta = previousTime != null;

        checkpointTimerText.text = TimeToString(checkpointTime.time);
        if (showDelta)
        {
            string deltaString = checkpointTime.time > previousTime.time ? "+" : "-";
            checkpointDeltaText.text = deltaString + TimeToString(checkpointTime.time - previousTime.time);
            checkpointDeltaText.color = checkpointTime.time > previousTime.time ? Color.red : Color.green;
        }

        checkpointTimer.SetActive(true);
        if (showDelta) checkpointDelta.SetActive(true);

        yield return new WaitForSeconds(5.0f);

        checkpointTimer.SetActive(false);
        if (showDelta) checkpointDelta.SetActive(false);

        showCheckpoint = null;
    }

    public void RefreshCheckpointTimes() // When reseting level from a victory
    {

        if (Goal.hasFinished && gameTime < bestTime)
        {
            bestTime = gameTime;
            previousCheckpointTimes = checkpointTimes;
        }
        checkpointTimes.Clear();
    }

    public void StoreTimes() // When exiting level
    {
        RefreshCheckpointTimes();

        SerializedLevelData currentLevel = LevelManager.instance.currentLevelData;

        if (currentLevel.bestTime > bestTime)
        {
            currentLevel.bestTime = bestTime;
            currentLevel.checkpointTimesOnPB = previousCheckpointTimes;
        }
    }

    public void LoadTimes()
    {
        if (LevelManager.instance == null) return;
        SerializedLevelData currentLevel = LevelManager.instance.currentLevelData;

        bestTime = currentLevel.bestTime;
        previousCheckpointTimes = currentLevel.checkpointTimesOnPB;
    }

    public string TimeToString(float time)
    {
        int floor = Mathf.FloorToInt(time);

        int minutes = floor / 60;
        int sec = floor % 60;
        int msec = Mathf.FloorToInt(1000*(time - floor));

        string m = minutes.ToString();
        string s = sec > 9 ? sec.ToString() : "0" + sec.ToString();
        string ms = "";
        if (msec < 100) ms += "0";
        if (msec <10) ms += "0";
        ms += msec.ToString();

        return m + ":" + s + ":" + ms;
    }

    private void InterruptShowCheckpoint()
    {
        if (showCheckpoint != null)
        {
            StopCoroutine(showCheckpoint);
            checkpointTimer.SetActive(false);
            checkpointDelta.SetActive(false);
        }
    }
}
