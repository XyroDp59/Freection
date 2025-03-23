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

    public void PlaySound(string soundId, float pitch = 1f)
    {
        soundsSource.pitch = pitch;
        soundsSource.volume = AudioControl.GeneralVolume * AudioControl.SoundsVolume;
        soundsSource.PlayOneShot(sounds[soundId]);
    }

    public void PlayMusic(string soundId)
    {
        musicSource.volume = AudioControl.GeneralVolume * AudioControl.MusicVolume;
        musicSource.PlayOneShot(musics[soundId]);
    }
}
