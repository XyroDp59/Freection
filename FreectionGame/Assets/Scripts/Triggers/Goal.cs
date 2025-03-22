using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    public static bool hasFinished;
    public UnityEvent onFinish;

    // Start is called before the first frame update
    void Start()
    {
        hasFinished = false;
    }

    public void Finish()
    {
        if (hasFinished) return;
        hasFinished = true;

        TimerManager.instance.StopTimer();
        LevelManager.instance.WinLevel();

        onFinish.Invoke();
    }

    public void OnTriggerEnter(Collider other)
    {
        PlayerControls player = other.GetComponent<PlayerControls>();
        if (player != null)
        {
            Finish();
        }
    }
}
