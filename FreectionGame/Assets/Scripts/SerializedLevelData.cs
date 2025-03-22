using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedLevelData
{
    public string levelSceneName;
    public float bestTime = float.MaxValue;
    public List<CheckpointTime> checkpointTimesOnPB = new List<CheckpointTime>();

    public bool unlocked = true;
    public bool isWon = false;


    public static SerializedLevelData LoadLevelData(string levelName)
    {
        if (PlayerPrefs.HasKey(levelName))
        {
            string json = PlayerPrefs.GetString(levelName);
            return JsonUtility.FromJson<SerializedLevelData>(json);
        }
        else
        {
            SerializedLevelData newLevelData = new SerializedLevelData();
            newLevelData.levelSceneName = levelName;
            newLevelData.bestTime = float.MaxValue;

            return newLevelData;
        }
    }

    public static void SaveLevelData(string levelName, SerializedLevelData levelData)
    {
        string json = JsonUtility.ToJson(levelData);
        PlayerPrefs.SetString(levelName, json);
    }
}
