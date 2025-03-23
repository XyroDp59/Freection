using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowVolume : MonoBehaviour
{
    [SerializeField] private float slowDownRate = 0.99f;

    private PlayerControls player;

    private void OnTriggerEnter(Collider other)
    {
        if (player != null) return;
        player = other.GetComponent<PlayerControls>();
        player.postProcessController.EnterSlowZone();
    }

    void OnTriggerStay(Collider other)
    {
        if (player == null) 
        {
            player = other.GetComponent<PlayerControls>();
        }

        if (player == null) return;
        player.rb.velocity *= slowDownRate;
    }

    private void OnTriggerExit(Collider other)
    {
        player.postProcessController.ExitSlowZone();
        if (player != null)
        {
            if (other.GetComponent<PlayerControls>() != null)
            {
                player = null;
            }
        }

    }
}
