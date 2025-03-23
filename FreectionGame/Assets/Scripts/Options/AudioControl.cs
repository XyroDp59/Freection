using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioControl : MonoBehaviour
{
    public static float GeneralVolume = 0.8f;
    public static float MusicVolume = 0.6f;
    public static float SoundsVolume = 0.8f;

    [SerializeField] Slider generalSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundsSlider;

    void Awake()
    {
        generalSlider.value = GeneralVolume;
        musicSlider.value = MusicVolume;
        soundsSlider.value = SoundsVolume;
    }

    public void SetGeneralVol(float newVol)
    {
        GeneralVolume = newVol;
        AudioManager.Instance.SetMusicVolume();
    }

    public void SetMusicVol(float newVol)
    {
        MusicVolume = newVol;
        AudioManager.Instance.SetMusicVolume();
    }

    public void SetSoundsVol(float newVol)
    {
        SoundsVolume = newVol;
    }
}
