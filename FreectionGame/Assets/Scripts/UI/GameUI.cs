using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject endGame;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Button resetButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button pauseResetButton;
    [SerializeField] Button pauseExitButton;


    [SerializeField] TextMeshProUGUI currentRunTime;
    [SerializeField] TextMeshProUGUI recordTime;
    [SerializeField] GameObject newRecord;

    [Header("RespawnAnim")]
    [SerializeField] RectTransform deathTransitionImage;
    [SerializeField] GameObject respawnRenderTex;
    [SerializeField] Rigidbody bounceBall;

    public static GameUI instance;

    GameObject lastSelectedGO;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {   
        lastSelectedGO = EventSystem.current.currentSelectedGameObject;

        resetButton.onClick.AddListener(() => LevelManager.instance.ResetLevel(PlayerControls.Instance));
        resetButton.onClick.AddListener(() => AudioManager.PlaySound("MenuValidate"));

        exitButton.onClick.AddListener(() => LevelManager.instance.ExitLevel(true));
        exitButton.onClick.AddListener(() => AudioManager.PlaySound("MenuValidate"));

        pauseResetButton.onClick.AddListener(() => LevelManager.instance.ResetLevel(PlayerControls.Instance));
        pauseResetButton.onClick.AddListener(() => AudioManager.PlaySound("MenuValidate"));
        pauseExitButton.onClick.AddListener(() => LevelManager.instance.ExitLevel(true));
        pauseExitButton.onClick.AddListener(() => AudioManager.PlaySound("MenuValidate"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowEndScreen(bool show)
    {
        TimerManager.instance.Show(!show);
        endGame.SetActive(show);

        if (show)
        {
            ShowPauseMenu(false);
            float currentTime = TimerManager.instance.GetLevelTime().Key;
            float bestTime = TimerManager.instance.GetLevelTime().Value;

            currentRunTime.text = TimerManager.TimeToString(currentTime);
            recordTime.text = bestTime > 99 ? "-:--:---" : TimerManager.TimeToString(bestTime);
            newRecord.SetActive(currentTime < bestTime);
        }
    }

    public void ShowDeathScreen(float fillT, float waitT, float removeT)
    {
        StartCoroutine(DeathScreenCor(fillT, waitT, removeT));
    }

    IEnumerator DeathScreenCor(float fillT, float waitT, float removeT)
    {
        var timeElapsed = 0f;
        deathTransitionImage.anchorMin = Vector2.right;
        deathTransitionImage.anchorMax = Vector2.one + Vector2.right;

        var ball = Instantiate(bounceBall.gameObject);
        ball.SetActive(true);
        ball.GetComponent<Rigidbody>().velocity = 20 * new Vector3(-2,-1f,0);

        while (timeElapsed < fillT)
        {
            deathTransitionImage.anchorMin = Vector2.Lerp(deathTransitionImage.anchorMin, Vector2.zero, .1f);
            deathTransitionImage.anchorMax = deathTransitionImage.anchorMin + Vector2.one;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        deathTransitionImage.anchorMin = Vector2.zero;
        deathTransitionImage.anchorMax = deathTransitionImage.anchorMin + Vector2.one;
        yield return new WaitForSeconds(waitT);

        timeElapsed = 0f;
        while (timeElapsed < removeT)
        {
            deathTransitionImage.anchorMin = Vector2.Lerp(deathTransitionImage.anchorMin, -Vector2.right, .1f);
            deathTransitionImage.anchorMax = deathTransitionImage.anchorMin + Vector2.one;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        deathTransitionImage.anchorMin = -Vector2.right;
        deathTransitionImage.anchorMax = deathTransitionImage.anchorMin + Vector2.one;
        Destroy(ball);
    }

    public void ShowPauseMenu(bool show)
    {
        AudioManager.PlaySound(show ? "PauseMenuOpen" : "PauseMenuClose");
        pauseMenu.SetActive(show);
        TimerManager.instance.PauseTimer(show);
        Time.timeScale = show ? 0.0f : 1.0f;
    }

    public void TogglePause()
    {
        ShowPauseMenu(!pauseMenu.activeSelf);
    }
}
