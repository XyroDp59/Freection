using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject levelButtonPrefab;

    [SerializeField] GameObject levelButtonParents;
    List<GameObject> levelButtons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            GameObject newButton = Instantiate(levelButtonPrefab, levelButtonParents.transform);

            Button button = newButton.GetComponent<Button>();
            button.image.sprite = levelData.thumbnail;
            button.onClick.AddListener(() => LevelManager.instance.LoadLevel(levelData.levelSceneName));

            levelButtons.Add(newButton);
        }
    }
}
