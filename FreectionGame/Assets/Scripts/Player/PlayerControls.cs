using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerControls : MonoBehaviour
{
    public static PlayerControls Instance;

    [Header("References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform visualSphere;
    [SerializeField] SphereCollider sphereCollider;
    [Header("Physics")]
    [SerializeField] float moveSpeed;
    [SerializeField] PhysicMaterial bounceMat;
    [SerializeField] PhysicMaterial nofrictionMat;
    [Header("Jump")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float jumpDelay;
    [SerializeField] float jumpStrength;


    public Rigidbody rb;

    Coroutine isDying = null;

    Inputs inputs;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction bounceAction;
    InputAction grappleAction;
    InputAction boostAction;
    InputAction resetCheckpointAction;
    InputAction resetLevelAction;
    InputAction pauseMenuAction;

    public UnityEvent onSpawn;
    public UnityEvent onRespawn;
    public UnityEvent onDeath;

    public bool isGrounded;
    bool canJump;

    void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;

        rb = GetComponent<Rigidbody>();

        canJump = true;

        inputs = new Inputs();
        moveAction = inputs.Player.Move;
        jumpAction = inputs.Player.Jump;
        bounceAction = inputs.Player.Bounce;
        grappleAction = inputs.Player.Grapple;
        boostAction = inputs.Player.Boost;
        resetCheckpointAction = inputs.Player.ResetCheckpoint;
        resetLevelAction = inputs.Player.ResetLevel;
        pauseMenuAction = inputs.Player.PauseMenu;

        jumpAction.performed += (_) => Jump();
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

    /*void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position - Vector3.up * (0.05f + sphereCollider.radius * 0.075f), sphereCollider.radius * 0.925f);
    }*/

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position - Vector3.up * (0.05f + sphereCollider.radius * 0.075f), sphereCollider.radius * 0.925f, groundLayer, QueryTriggerInteraction.UseGlobal);

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 forwardVec = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up);
        Vector3 rightVec = Vector3.ProjectOnPlane(playerCamera.transform.right, Vector3.up);
        Vector3 realMove = Time.deltaTime * moveSpeed * (rightVec * moveInput.x + forwardVec * moveInput.y);

        Vector2 velAxis = Vector2.Perpendicular(new Vector2(rb.velocity.x, rb.velocity.z)).normalized;

        visualSphere.Rotate(new Vector3(-velAxis.x, 0, -velAxis.y), rb.velocity.magnitude * Time.deltaTime * 50f, Space.World);

        rb.AddForce(realMove, ForceMode.Force);
    }

    public void Jump()
    {
        if (!canJump || !isGrounded) return;

        canJump = false;
        rb.AddForce(jumpStrength * Vector3.up);
        StartCoroutine(lockJumpCor());
    }

    IEnumerator lockJumpCor()
    {
        yield return new WaitForSeconds(jumpDelay);
        canJump = true;
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
