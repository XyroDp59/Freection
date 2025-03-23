using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnball;
    [SerializeField] private float timeSpawn;
    private bool spawnable = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private IEnumerator Spawn()
    {
        Instantiate(spawnball,transform.position,transform.rotation);
        yield return new WaitForSeconds(timeSpawn);
        spawnable = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (spawnable)
        {
            spawnable = false;
            StartCoroutine(Spawn());
        }
    }
}
