using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] string musicName;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.PlayMusic(musicName);   
    }
}
