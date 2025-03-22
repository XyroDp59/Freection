using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCamera : MonoBehaviour
{
    CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
       
    }

    private void Start()
    {
        PlayerControls.Instance.onLevelReset.AddListener(() => ActivateGoalCamera(false));
    }

    public void ActivateGoalCamera(bool isActive)
    {
        if (isActive && !gameObject.activeSelf)
        {
            if (_virtualCamera == null) _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.LookAt = PlayerControls.Instance.transform;
            PlayerCamera.Instance.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        if (!isActive && gameObject.activeSelf)
        {
            PlayerCamera.Instance.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
