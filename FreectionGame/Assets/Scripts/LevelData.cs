using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObject/LevelData", order = 0)]
public class LevelData : ScriptableObject
{
    public string levelSceneName;
    public float bestTime;
    public Texture2D thumbnail;
    public bool unlocked;
}
