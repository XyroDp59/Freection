using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour
{
    [SerializeField] Vector3 velocity;

    private void OnTriggerEnter(Collider other)
    {
        //SetPlayer location and velocity
        PlayerControls player = other.GetComponent<PlayerControls>();
        if (player == null) return;

        player.transform.SetPositionAndRotation(transform.position, transform.rotation);
        player.rb.velocity = velocity;
    }
}
