using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckpointManager : MonoBehaviour
{
    [NonSerialized]
    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    [NonSerialized]
    public Checkpoint spawnPoint = null;
    [NonSerialized]
    public Checkpoint currentCheckpoint = null;

    public static CheckpointManager instance;

    void Awake()
    {
        if (instance != null) 
        {
            Destroy(this);
        }
        else 
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentCheckpoint = checkpoints[0];
    }
}
