using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class VideoControl : OptionPanel
{
    [SerializeField] Toggle fullscreenToggle;
    Resolution currentResolution;
    [SerializeField] TMP_Dropdown resolutionsDropdown;
    Resolution oldRes;
    bool oldFullscreen;
    [SerializeField] float timeBeforeTurnBack;
    Coroutine issueCoroutine;
    [SerializeField] GameObject confirmationMenu;
    [SerializeField] TextMeshProUGUI countdown;

    bool isRevertingChanges = false;

    void Awake()
    {
        List<string> options = new List<string>();
        resolutionsDropdown.ClearOptions();
        foreach (Resolution resolution in Screen.resolutions)
        {
            if (currentResolution.height == resolution.height && currentResolution.width == resolution.width && 
                currentResolution.refreshRate == resolution.refreshRate)
                resolutionsDropdown.value = options.Count;
            options.Add(resolution.width + "x" + resolution.height + ", " + resolution.refreshRate + "Hz");
        }
        resolutionsDropdown.AddOptions(options);

        LoadPrefs();
    }

    public void ChangeResolution()
    {
        if (isRevertingChanges) return;
        oldRes.width = Screen.currentResolution.width;
        oldRes.height = Screen.currentResolution.height;
        oldRes.refreshRate = Screen.currentResolution.refreshRate;
        oldFullscreen = Screen.fullScreen;
        currentResolution = Screen.resolutions[resolutionsDropdown.value];
        Screen.SetResolution(currentResolution.width, currentResolution.height, fullscreenToggle.isOn, currentResolution.refreshRate);
        confirmationMenu.SetActive(true);
        issueCoroutine = StartCoroutine(changeBackIfIssue());
    }

    public void ChangeBackResolution()
    {
        isRevertingChanges = true;
        if (issueCoroutine != null)
            StopCoroutine(issueCoroutine);
        confirmationMenu.SetActive(false);
        currentResolution = oldRes;
        fullscreenToggle.isOn = oldFullscreen;
        for (int i = 0; i < Screen.resolutions.Length; ++i)
        {
            if (Screen.resolutions[i].height == currentResolution.height &&
                Screen.resolutions[i].width == currentResolution.width &&
                Screen.resolutions[i].refreshRate == currentResolution.refreshRate)
            {
                resolutionsDropdown.value = i;
                break;
            }
        }
        Screen.SetResolution(currentResolution.width, currentResolution.height, fullscreenToggle.isOn, currentResolution.refreshRate);

        isRevertingChanges = false;
    }

    public void ConfirmResolution()
    {
        if (issueCoroutine != null)
            StopCoroutine(issueCoroutine);
        confirmationMenu.SetActive(false);
        PlayerPrefs.SetInt("ScreenWidth", currentResolution.width);
        PlayerPrefs.SetInt("ScreenHeight", currentResolution.height);
        PlayerPrefs.SetInt("ScreenRefreshRate", currentResolution.refreshRate);
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
    }

    IEnumerator changeBackIfIssue()
    {
        int seconds = (int)timeBeforeTurnBack;
        countdown.text = seconds.ToString();
        while (seconds >= 0)
        {
            yield return new WaitForSecondsRealtime(1);
            seconds--;
            countdown.text = seconds.ToString();
        }
        ChangeBackResolution();
        issueCoroutine = null;
    }

    override public void LoadPrefs()
    {
        base.LoadPrefs();

        bool isFullScreen = true;
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            isFullScreen = PlayerPrefs.GetInt("Fullscreen") == 0;
        }
        fullscreenToggle.isOn = isFullScreen;

        if (PlayerPrefs.HasKey("ScreenHeight"))
        {
            currentResolution.height = PlayerPrefs.GetInt("ScreenHeight");
            currentResolution.width = PlayerPrefs.GetInt("ScreenWidth");
            currentResolution.refreshRate = PlayerPrefs.GetInt("ScreenRefreshRate");
            for (int i = 0; i < Screen.resolutions.Length; ++i)
            {
                if (Screen.resolutions[i].height == currentResolution.height &&
                    Screen.resolutions[i].width == currentResolution.width &&
                    Screen.resolutions[i].refreshRate == currentResolution.refreshRate)
                {
                    resolutionsDropdown.value = i;
                    break;
                }
            }
        }
        else
        {
            currentResolution = Screen.resolutions[Screen.resolutions.Length - 1];
            resolutionsDropdown.value = Screen.resolutions.Length - 1;
        }

        //Screen.SetResolution(currentResolution.width, currentResolution.height, fullscreenToggle.isOn, currentResolution.refreshRate);
    }
}
