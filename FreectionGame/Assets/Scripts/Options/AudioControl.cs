using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioControl : OptionPanel
{
    public static float GeneralVolume = 0.8f;
    public static float MusicVolume = 0.6f;
    public static float SoundsVolume = 0.8f;

    [SerializeField] Slider generalSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundsSlider;

    void Awake()
    {
        LoadPrefs();

        generalSlider.value = GeneralVolume;
        musicSlider.value = MusicVolume;
        soundsSlider.value = SoundsVolume;
    }

    public void SetGeneralVol(float newVol)
    {
        GeneralVolume = newVol;
        AudioManager.Instance.SetMusicVolume();
        PlayerPrefs.SetFloat("GeneralVolume", GeneralVolume);
    }

    public void SetMusicVol(float newVol)
    {
        MusicVolume = newVol;
        AudioManager.Instance.SetMusicVolume();
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
    }

    public void SetSoundsVol(float newVol)
    {
        SoundsVolume = newVol;
        PlayerPrefs.SetFloat("SoundsVolume", SoundsVolume);
    }

    override public void LoadPrefs()
    {
        base.LoadPrefs();

        if (PlayerPrefs.HasKey("GeneralVolume"))
        {
            GeneralVolume = PlayerPrefs.GetFloat("GeneralVolume");
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }

        if (PlayerPrefs.HasKey("SoundsVolume"))
        {
            SoundsVolume = PlayerPrefs.GetFloat("SoundsVolume");
        }

        AudioManager.Instance.SetMusicVolume();
    }
}
