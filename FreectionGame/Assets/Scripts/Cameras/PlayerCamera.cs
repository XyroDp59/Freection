using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static Cinemachine.CinemachineCore;

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


    float sensitivityX;
    float sensitivityY;
    static float inputX;
    static float inputY;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(Instance);

        CinemachineCore.GetInputAxis = PlayerCamera.GetAxis;

        if (PlayerPrefs.HasKey("sensitivity"))
        {
            sensitivityX = PlayerPrefs.GetFloat("sensitivity");
            sensitivityY = sensitivityX;
        }
        else
        {
            sensitivityX = 1.0f;
            sensitivityY = 1.0f;
        }
        if (PlayerPrefs.HasKey("sensitivityXY"))
        {
            float ratio = PlayerPrefs.GetFloat("sensitivityXY");
            sensitivityX *= ratio;
        }

        freeLook = GetComponent<CinemachineFreeLook>();

        inputs = new Inputs();
        zoomAction = inputs.Player.Zoom;
        cameraXY = inputs.Player.Look;
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

        //Custom look input
        if (IsKeyboardOrMouse())
        {
            MouseLook();
        }
        else
        {
            ControllerLook();
        }
    }

    public bool IsKeyboardOrMouse()
    {
        bool hasGamepad = Gamepad.current != null;
        return !hasGamepad;
        InputDevice activeDevice = cameraXY.activeControl.device;
        return activeDevice.description.deviceClass.Equals("Keyboard") || activeDevice.description.deviceClass.Equals("Mouse");
    }


    private void MouseLook()
    {
        Vector2 xyinput = cameraXY.ReadValue<Vector2>();
        //freeLook.m_XAxis.m_InputAxisValue = xyinput.x * 0.1f;
        //freeLook.m_YAxis.m_InputAxisValue = xyinput.y * 0.1f;

        inputX = xyinput.x * sensitivityX;
        inputY = xyinput.y * sensitivityY;
    }

    private void ControllerLook()
    {
        Vector2 xyinput = cameraXY.ReadValue<Vector2>();
        inputX = xyinput.x * sensitivityX;
        inputY = xyinput.y * sensitivityY;
    }

    public static float GetAxis(string axisName)
    {
        if (axisName == "Mouse Y") return inputY;
        else if (axisName == "Mouse X") return inputX;
        else return Input.GetAxis(axisName);
    }
}
