using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    public static bool hasFinished;
    public UnityEvent onFinish;

    public GoalCamera goalCamera;

    // Start is called before the first frame update
    void Start()
    {
        hasFinished = false;
    }

    public void Finish()
    {
        if (hasFinished) return;

        LevelManager.instance.WinLevel();
        goalCamera.ActivateGoalCamera(true);

        onFinish.Invoke();
    }

    public void OnTriggerEnter(Collider other)
    {
        PlayerControls player = other.GetComponent<PlayerControls>();
        if (player != null && !player.IsDying())
        {
            Finish();
        }
    }
}
