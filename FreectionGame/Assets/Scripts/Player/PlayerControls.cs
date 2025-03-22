using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] PhysicMaterial bounceMat;
    [SerializeField] PhysicMaterial nofrictionMat;


    public Rigidbody rb;
    SphereCollider sphereCollider;

    Coroutine isDying = null;

    Inputs inputs;
    InputAction moveAction;
    InputAction bounceAction;
    InputAction grappleAction;
    InputAction boostAction;
    InputAction resetCheckpointAction;
    InputAction resetLevelAction;
    InputAction pauseMenuAction;

    public UnityEvent onSpawn;
    public UnityEvent onRespawn;
    public UnityEvent onDeath;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        inputs = new Inputs();
        moveAction = inputs.Player.Move;
        bounceAction = inputs.Player.Bounce;
        grappleAction = inputs.Player.Grapple;
        boostAction = inputs.Player.Boost;
        resetCheckpointAction = inputs.Player.ResetCheckpoint;
        resetLevelAction = inputs.Player.ResetLevel;
        pauseMenuAction = inputs.Player.PauseMenu;

        bounceAction.performed += (_) => SwitchBounce(true);
        bounceAction.canceled += (_) => SwitchBounce(false);
        grappleAction.performed += (_) => UseGrapple();
        grappleAction.canceled += (_) => ReleaseGrapple();
        boostAction.performed += (_) => Boost();
        resetCheckpointAction.performed += (_) => Respawn(CheckpointManager.instance.currentCheckpoint, false);
        resetLevelAction.performed += (_) => Respawn(CheckpointManager.instance.spawnPoint, false);
        pauseMenuAction.performed += (_) => OpenPauseMenu();

        inputs.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 realMove = Time.deltaTime * moveSpeed * (transform.rotation * new Vector3(moveInput.x, 0, moveInput.y));

        rb.AddForce(realMove, ForceMode.Force);
    }

    public void SwitchBounce(bool nowBouncing)
    {
        sphereCollider.material = nowBouncing ? bounceMat : nofrictionMat;
    }

    public void UseGrapple()
    {

    }

    public void ReleaseGrapple()
    {

    }

    public void Boost()
    {

    }

    public void Spawn()
    {
        Checkpoint spawn = CheckpointManager.instance.spawnPoint;
        CheckpointManager.instance.currentCheckpoint = spawn;
        transform.SetPositionAndRotation(spawn.respawnAnchor.transform.position, spawn.respawnAnchor.transform.rotation);

        onSpawn.Invoke();
    }

    public void Respawn(Checkpoint checkpoint, bool keepVel)
    {
        transform.SetPositionAndRotation(checkpoint.respawnAnchor.transform.position, checkpoint.respawnAnchor.transform.rotation);

        if (keepVel)
        {
            rb.velocity = checkpoint.velocity;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        onRespawn.Invoke();
    }

    public void OpenPauseMenu()
    {

    }

    public void Die()
    {
        if (isDying != null) return;
        isDying = StartCoroutine(PlayDeathAnimation());
        onDeath.Invoke();
    }

    private IEnumerator PlayDeathAnimation()
    {
        yield return new WaitForSeconds(2.0f);

        Respawn(CheckpointManager.instance.currentCheckpoint, false);
        isDying = null;
    }
}
