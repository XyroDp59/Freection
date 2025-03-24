using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Menu_Background : MonoBehaviour
{
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequence = 4f;
    [SerializeField] private Vector3 axe;
    private Vector3 basepos;
    // Start is called before the first frame update
    void Start()
    {
        basepos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = basepos + amplitude*Mathf.Sin(Time.time*frequence)*axe.normalized;
    }
}
