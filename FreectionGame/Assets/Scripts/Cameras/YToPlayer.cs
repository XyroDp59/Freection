using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YToPlayer : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float YOffset;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, player.position.y + YOffset, transform.position.z);
    }
}
