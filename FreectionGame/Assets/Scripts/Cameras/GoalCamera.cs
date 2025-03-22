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

    public void ActivateGoalCamera()
    {
        PlayerCamera.Instance.gameObject.SetActive(false);
        gameObject.SetActive(true);
        Debug.Log("bdofoqnf");
    }
}
