using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{

    [SerializeField] GameObject PanelsParent;
    [SerializeField] GameObject PanelsButtonParent;
    [SerializeField] GameObject PanelsShadowParent;

    List<GameObject> Panels = new List<GameObject>();
    List<Button> PanelButtons = new List<Button>();
    List<Image> PanelsShadows = new List<Image>();
    List<float> PanelsShadowsBaseAlpha = new List<float>();

    bool isInit = false;
    private int currentPanelIndex = -1;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (isInit) return;
        isInit = true;

        for (int i = 0; i < PanelsParent.transform.childCount; ++i)
        {
            Panels.Add(PanelsParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < PanelsButtonParent.transform.childCount; ++i)
        {
            PanelButtons.Add(PanelsButtonParent.transform.GetChild(i).gameObject.GetComponent<Button>());
        }

        for (int i = 0; i < PanelsShadowParent.transform.childCount; ++i)
        {
            PanelsShadows.Add(PanelsShadowParent.transform.GetChild(i).gameObject.GetComponent<Image>());
            PanelsShadowsBaseAlpha.Add(PanelsShadows[i].color.a);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowOptions()
    {
        Init();
        ShowPanel(0);
        gameObject.SetActive(true);
    }

    public void ExitOptions()
    {
        currentPanelIndex = -1;
        gameObject.SetActive(false);
    }

    public void ShowPanel(int index)
    {
        if (index == currentPanelIndex) return;
        currentPanelIndex = index;

        for (int i = 0; i < Panels.Count; ++i)
        {
            Panels[i].SetActive(i == index);
        }
        for (int i = 0; i < PanelButtons.Count; ++i)
        {
            PanelButtons[i].interactable = (i != index);
        }
        for (int i = 0; i < PanelsShadows.Count; ++i)
        {
            PanelsShadows[i].color = new Color(PanelsShadows[i].color.r, PanelsShadows[i].color.g, PanelsShadows[i].color.b, (i == index) ? PanelsShadowsBaseAlpha[i] : 0.0f);
        }
    }

    public void LoadPrefs()
    {
        Init();
        for (int i = 0; i < Panels.Count; ++i)
        {
            OptionPanel panel = Panels[i].GetComponent<OptionPanel>();
            panel.LoadPrefs();
        }
    }
}
