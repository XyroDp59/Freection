using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{
    [SerializeField] float force;
    [SerializeField] float gravPow = 2f;
    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out var rb))
        {
            float d = Vector3.Distance(other.transform.position, transform.position);
            rb.AddForce(force/Mathf.Pow(d, gravPow)
                * (transform.position - other.transform.position).normalized);
        }   
    }
}
