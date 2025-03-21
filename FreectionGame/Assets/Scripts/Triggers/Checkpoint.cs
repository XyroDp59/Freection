using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("Starts at 0, should increase as the player gets further in the level. They should be unique numbers")]
    public int order;

    public GameObject respawnAnchor;

    public UnityEvent onReachCheckpoint;

    public Vector3 position;
    public Vector3 velocity;

    public Vector3 cameraPosition;
    public Quaternion cameraRotation;

    [SerializeField] Material inactiveMaterial;
    [SerializeField] Material activeMaterial;

    // Start is called before the first frame update
    void Start()
    {
        onReachCheckpoint.AddListener(() => TimerManager.instance.ReachCheckpoint(this));

        CheckpointManager.instance.checkpoints.Add(this);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (CheckpointManager.instance.currentCheckpoint == this) return;

        PlayerControls player = other.GetComponent<PlayerControls>();
        if (player == null || player.blockGameInputs) return;

        if (CheckpointManager.instance.currentCheckpoint == null)
        {
            CheckpointManager.instance.currentCheckpoint = this;
        }
        else if (CheckpointManager.instance.currentCheckpoint.order <= order)
        {
            CheckpointManager.instance.currentCheckpoint = this;
            position = player.transform.position;
            velocity = player.rb.velocity;

            cameraPosition = Camera.main.transform.position;
            cameraRotation = Camera.main.transform.rotation;

            onReachCheckpoint.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(respawnAnchor.transform.position, 0.5f);
        Gizmos.DrawRay(respawnAnchor.transform.position, respawnAnchor.transform.forward);
    }
}
