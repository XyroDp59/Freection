using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject levelButtonPrefab;
    [SerializeField] Image BackgroundGradient;

    [SerializeField] GameObject levelButtonParents;
    List<GameObject> levelButtons = new List<GameObject>();

    [SerializeField] private Color gradientColor1;
    [SerializeField] private Color gradientColor2;

    [SerializeField] float gradientAmplitude = 1.0f;
    [SerializeField] float gradientPeriod = 1.0f;

    [SerializeField] Color wonColor;

    [SerializeField] UnityEvent onMenuLoaded;

    GameObject lastSelectedGO;

    // Start is called before the first frame update
    void Start()
    {
        onMenuLoaded.Invoke();
        AudioManager.PlayMusic("Fading");
    }

    // Update is called once per frame
    void Update()
    {   
        lastSelectedGO = EventSystem.current.currentSelectedGameObject;
        BackgroundGradient.color = Color.Lerp(gradientColor1, gradientColor2, 0.5f + 0.5f * gradientAmplitude * Mathf.Sin(Time.time * gradientPeriod));
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void RefreshLevels()
    {
        foreach(GameObject go in levelButtons)
        {
            Destroy(go);
        }

        foreach (LevelData levelData in LevelManager.instance.levels)
        {
            SerializedLevelData storedLevelData = SerializedLevelData.LoadLevelData(levelData.levelSceneName);

            GameObject newButton = Instantiate(levelButtonPrefab, levelButtonParents.transform);
            LevelButton button = newButton.GetComponentInChildren<LevelButton>();

            button.button.image.sprite = levelData.thumbnail;
            button.button.onClick.AddListener(() => LevelManager.instance.LoadLevel(levelData.levelSceneName));

            if (storedLevelData.isWon)
            {
                button.time.text = TimerManager.TimeToString(storedLevelData.bestTime);
            }

            button.Border.color = storedLevelData.isWon ? wonColor : Color.black;
            button.Locked.color = new Color(button.Locked.color.r, button.Locked.color.g, button.Locked.color.b, storedLevelData.unlocked ? 0.0f : 1.0f);

            levelButtons.Add(newButton);
        }
    }
}
