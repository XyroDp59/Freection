using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopperTracking : MonoBehaviour
{
    [SerializeField] Transform dolly;
    [SerializeField] Transform player;
    [SerializeField] float scalePow = 0.3f;
    [SerializeField] float posPow = 0.85f;

    // Update is called once per frame
    void Update()
    {
        dolly.transform.localPosition = Mathf.Pow(player.position.y, posPow) * Vector3.up;
        dolly.transform.localScale = Mathf.Pow(player.position.y, scalePow) * Vector3.one;
    }
}
