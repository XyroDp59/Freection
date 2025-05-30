using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObject/LevelData", order = 0)]
public class LevelData : ScriptableObject
{
    public string levelSceneName;
    public float bestTime = float.MaxValue;
    public List<CheckpointTime> checkpointTimesOnPB;

    public Sprite thumbnail;
    public bool unlocked = true;
    public bool isWon = false;
}
