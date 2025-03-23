using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{
    public static float GeneralVolume = 0.8f;
    public static float MusicVolume = 0.8f;
    public static float SoundsVolume = 0.8f;
    [SerializeField] AudioSource mainMenuMusic;
    
    public void SetGeneralVol(float newVol)
    {
        GeneralVolume = newVol;
        mainMenuMusic.volume = GeneralVolume * MusicVolume;
    }

    public void SetMusicVol(float newVol)
    {
        MusicVolume = newVol;
        mainMenuMusic.volume = GeneralVolume * MusicVolume;
    }

    public void SetSoundsVol(float newVol)
    {
        SoundsVolume = newVol;
    }
}
