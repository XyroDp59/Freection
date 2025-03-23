using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource soundsSource;
    [SerializeField] TDictionary<string, AudioClip> musics;
    
    [SerializeField] TDictionary<string, AudioClip> sounds;

    private void Awake() {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void PlaySound(string soundId)
    {
        Instance.soundsSource.pitch = 1;
        Instance.soundsSource.volume = AudioControl.GeneralVolume * AudioControl.SoundsVolume;
        Instance.soundsSource.PlayOneShot(Instance.sounds[soundId]);
    }

    public static void PlaySound(string soundId, float pitch)
    {
        Instance.soundsSource.pitch = pitch;
        Instance.soundsSource.volume = AudioControl.GeneralVolume * AudioControl.SoundsVolume;
        Instance.soundsSource.PlayOneShot(Instance.sounds[soundId]);
    }

    public static void PlayMusic(string soundId)
    {
        Instance.musicSource.volume = AudioControl.GeneralVolume * AudioControl.MusicVolume;
        Instance.musicSource.PlayOneShot(Instance.musics[soundId]);
    }
}
