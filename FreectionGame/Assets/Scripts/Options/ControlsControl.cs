using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsControl : MonoBehaviour
{
    public static float sensitivity = 0.5f;
    public static float sensitivityXY = 0.5f;


    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider sensitivityXYSlider;

    private void Awake()
    {
        sensitivitySlider.value = sensitivity;
        sensitivityXYSlider.value = sensitivityXY;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
