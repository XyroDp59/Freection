using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }
    private CinemachineFreeLook freeLook;
    [Header("Zoom")]
    [SerializeField] private float minZoomFOV = 12f;
    [SerializeField] private float maxZoomFOV = 70f;
    [SerializeField] private AnimationCurve curveZoomFOV;
    private float zoomFOV = 30f;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(Instance);

        freeLook = GetComponent<CinemachineFreeLook>();
    }

    public void SetCamPosition(Vector3 position)
    {
        Quaternion rotation = Quaternion.identity;
        freeLook.ForceCameraPosition(position, rotation);
    }

    public void SetCamZoom(float t)
    {
        t = 1 - t;
        t = Mathf.Clamp01(t);
        zoomFOV = minZoomFOV + curveZoomFOV.Evaluate(t) * (maxZoomFOV - minZoomFOV);
        freeLook.m_Lens.FieldOfView = zoomFOV;
    }
}
