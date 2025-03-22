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
        _virtualCamera.LookAt = PlayerControls.Instance.transform;
    }

    private void Start()
    {
        PlayerControls.Instance.onLevelReset.AddListener(() => ActivateGoalCamera(false));
    }

    public void ActivateGoalCamera(bool isActive)
    {
        PlayerCamera.Instance.gameObject.SetActive(!isActive);
        gameObject.SetActive(isActive);
    }
}
