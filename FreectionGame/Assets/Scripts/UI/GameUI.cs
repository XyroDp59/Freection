using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject death;
    [SerializeField] GameObject endGame;
    [SerializeField] Button resetButton;
    [SerializeField] Button exitButton;

    public static GameUI instance;

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
        PlayerControls.Instance.onDeathStart.AddListener(() => ShowDeathScreen(true));
        PlayerControls.Instance.onDeathEnd.AddListener(() => ShowDeathScreen(false));

        resetButton.onClick.AddListener(() => LevelManager.instance.ResetLevel(PlayerControls.Instance));
        exitButton.onClick.AddListener(() => LevelManager.instance.ExitLevel(true));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowEndScreen(bool show)
    {
        endGame.SetActive(show);
    }

    public void ShowDeathScreen(bool show)
    {
        death.SetActive(show);
    }
}
