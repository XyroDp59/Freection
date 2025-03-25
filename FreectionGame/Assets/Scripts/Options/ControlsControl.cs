using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsControl : OptionPanel
{
    public static float sensitivity = 0.5f;
    public static float sensitivityXY = 0.5f;


    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider sensitivityXYSlider;

    private void Awake()
    {
        LoadPrefs();

        sensitivitySlider.value = sensitivity;
        sensitivityXYSlider.value = sensitivityXY;
    }

    public void SetSensitivity(float newSens)
    {
        sensitivity = newSens;
        PlayerPrefs.SetFloat("sensitivity", sensitivity);
    }

    public void SetSensitivityXYRatio(float newXY)
    {
        sensitivityXY = newXY;
        PlayerPrefs.SetFloat("sensitivityXY", sensitivityXY);
    }

    override public void LoadPrefs()
    {
        base.LoadPrefs();

        if (PlayerPrefs.HasKey("sensitivity"))
        {
            sensitivity = PlayerPrefs.GetFloat("sensitivity");
        }

        if (PlayerPrefs.HasKey("sensitivityXY"))
        {
            sensitivityXY = PlayerPrefs.GetFloat("sensitivityXY");
        }
    }
}
