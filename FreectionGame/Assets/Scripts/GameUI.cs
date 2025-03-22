using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
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
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
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
}
