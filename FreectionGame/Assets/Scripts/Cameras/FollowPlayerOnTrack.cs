using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerOnTrack : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _cam;

    private void Update()
    {
        _cam.transform.LookAt(PlayerControls.Instance.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerControls p;
        if (other.TryGetComponent(out p))
        {
            _cam.gameObject.SetActive(true);
            PlayerCamera.Instance.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerControls p;
        if (other.TryGetComponent(out p))
        {
            _cam.gameObject.SetActive(false);
            PlayerCamera.Instance.SetCamPosition(_cam.transform.position);
            PlayerCamera.Instance.gameObject.SetActive(true);
        }
    }
}
