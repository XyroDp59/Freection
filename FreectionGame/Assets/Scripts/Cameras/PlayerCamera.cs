using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }
    private CinemachineFreeLook freeLook;
    [Header("Zoom")]
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float zoomSpeed = 5f;
    private float wantedZoom = 5f;
    private float currentZoom = 5f;
    [SerializeField] private float baseBotHeight;
    [SerializeField] private float baseBotRad;
    [SerializeField] private float baseMidHeight;
    [SerializeField] private float baseMidRad;
    [SerializeField] private float baseTopHeight;
    [SerializeField] private float baseTopRad;

    [Header("FOV")]
    [SerializeField] private AnimationCurve curveFOV;
    [SerializeField] private float minFOV = 35f;
    [SerializeField] private float maxFOV = 75f;
    float FOV;

    private Inputs inputs;
    InputAction zoomAction;
    InputAction cameraXY;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(Instance);

        freeLook = GetComponent<CinemachineFreeLook>();

        inputs = new Inputs();
        zoomAction = inputs.Player.Zoom;
        inputs.Enable();
    }

    private void OnDestroy()
    {
        inputs.Dispose();
    }

    public void SetCamPosition(Vector3 position)
    {
        Quaternion rotation = Quaternion.identity;
        freeLook.ForceCameraPosition(position, rotation);
    }

    public void SetCamTransform(Vector3 position, Quaternion rotation)
    {
        freeLook.ForceCameraPosition(position, rotation);
    }


    public void SetCamZoom()
    {
        freeLook.m_Orbits[0].m_Height = baseBotHeight * currentZoom;
        freeLook.m_Orbits[0].m_Radius = baseBotRad * currentZoom;
        freeLook.m_Orbits[1].m_Height = baseMidHeight * currentZoom;
        freeLook.m_Orbits[1].m_Radius = baseMidRad * currentZoom;
        freeLook.m_Orbits[2].m_Height = baseTopHeight * currentZoom;
        freeLook.m_Orbits[2].m_Radius = baseTopRad * currentZoom;
    }

    private void Update() {
        
        float zoomDelta = zoomAction.ReadValue<float>();

        wantedZoom = Mathf.Clamp(wantedZoom + zoomDelta * Time.deltaTime * zoomSpeed, minZoom, maxZoom);
        currentZoom = Mathf.Lerp(currentZoom, wantedZoom, 0.05f);

        SetCamZoom();


        float v = PlayerControls.Instance.rb.velocity.magnitude / 50f;
        if (v < curveFOV.keys[curveFOV.length - 1].time)
            FOV = minFOV + (maxFOV-minFOV) * curveFOV.Evaluate(v);
        else FOV = minFOV + (maxFOV - minFOV) * curveFOV.keys[curveFOV.length - 1].value;
        freeLook.m_Lens.FieldOfView = FOV;

        //Custom XY axis
        Vector2 xyinput = cameraXY.ReadValue<Vector2>();
        freeLook.m_XAxis.Value += xyinput.x;
        freeLook.m_YAxis.Value += xyinput.y;
    }


}
