using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] float fadeDuration = 2f;
    [SerializeField] AudioSource musicSource1;
    [SerializeField] AudioSource musicSource2;
    [SerializeField] AudioSource soundsSource;
    [SerializeField] TDictionary<string, AudioClip> musics;
    
    [SerializeField] TDictionary<string, AudioClip> sounds;

    bool ms1;
    Coroutine fadeCor;

    private void Awake() {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            ms1 = true;
            DontDestroyOnLoad(gameObject);
        }
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

    public static void PlayMusic(string musicId)
    {
        if (Instance.ms1)
        {
            Instance.musicSource1.clip = Instance.musics[musicId];
            Instance.musicSource1.Play();
        }
        else
        {
            Instance.musicSource2.clip = Instance.musics[musicId];
            Instance.musicSource2.Play();
        }
        if (Instance.fadeCor != null)
            Instance.StopCoroutine(Instance.fadeCor);
        Instance.fadeCor = Instance.StartCoroutine(Instance.FadeVolume(Instance.ms1));
        Instance.ms1 = !Instance.ms1;
    }

    IEnumerator FadeVolume(bool isMs1)
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            musicSource1.volume = Mathf.Lerp(0, AudioControl.GeneralVolume * AudioControl.MusicVolume, isMs1 ? timeElapsed/fadeDuration : 1 - timeElapsed/fadeDuration);
            musicSource2.volume = Mathf.Lerp(0, AudioControl.GeneralVolume * AudioControl.MusicVolume, !isMs1 ? timeElapsed/fadeDuration : 1 - timeElapsed/fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        musicSource1.volume = isMs1 ? AudioControl.GeneralVolume * AudioControl.MusicVolume : 0;
        musicSource2.volume = !isMs1 ? AudioControl.GeneralVolume * AudioControl.MusicVolume : 0;
        if (isMs1)
            musicSource2.Stop();
        else
            musicSource1.Stop();
        fadeCor = null;
    }
}
