using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_Ball_Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.other.gameObject);
    }
}
