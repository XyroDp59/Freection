using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] Transform deformTransform;
    [SerializeField] SphereCollider sphereCollider;
    [Header("Physics")]
    [SerializeField] float moveSpeed;
    [SerializeField] PhysicMaterial bounceMat;
    [SerializeField] PhysicMaterial nofrictionMat;
    [Header("Jump")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float jumpDelay;
    [SerializeField] float jumpStrength;
    [Header("Visuals")]
    [SerializeField] AnimationCurve bounceDeformCurve;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] AnimationCurve trailLengthPerSpeedCurve;
    [SerializeField] float trailCurveLog;
    [SerializeField] float trailStartOffset;
    [SerializeField] float trailLengthLerp;

    public Rigidbody rb;

    Coroutine isDying = null;
    Coroutine deformCor;

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
    public UnityEvent onDeathStart;
    public UnityEvent onDeathEnd;

    public bool isGrounded;
    bool canJump;
    bool isBouncing;
    public bool blockGameInputs = false;

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
        resetCheckpointAction.performed += (_) => TryRespawn(CheckpointManager.instance.currentCheckpoint, false);
        resetLevelAction.performed += (_) => LevelManager.instance.ResetLevel(this);
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

        trailRenderer.time = Mathf.Lerp(trailRenderer.time, Mathf.Max(trailLengthPerSpeedCurve.Evaluate(Mathf.Log(rb.velocity.magnitude, trailCurveLog)) - trailStartOffset, 0), trailLengthLerp);
        trailRenderer.enabled = (trailLengthPerSpeedCurve.Evaluate(Mathf.Log(rb.velocity.magnitude, trailCurveLog)) - trailStartOffset) > 0;

        if (!blockGameInputs) rb.AddForce(realMove, ForceMode.Force);
    }

    public void Jump()
    {
        if (blockGameInputs || !canJump || !isGrounded) return;

        canJump = false;
        rb.AddForce(jumpStrength * Vector3.up);
        StartCoroutine(lockJumpCor());
        if (deformCor != null)
            StopCoroutine(deformCor);
        deformCor = StartCoroutine(DeformBallCoroutine(-0.5f * Vector3.up));
    }

    IEnumerator lockJumpCor()
    {
        yield return new WaitForSeconds(jumpDelay);
        canJump = true;
    }

    public void SwitchBounce(bool nowBouncing)
    {
        if (blockGameInputs) return;

        sphereCollider.material = nowBouncing ? bounceMat : nofrictionMat;
        isBouncing = nowBouncing;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isBouncing)
        {
            if (deformCor != null)
                StopCoroutine(deformCor);
            deformCor = StartCoroutine(DeformBallCoroutine(collision.impulse.normalized));
        }
    }

    IEnumerator DeformBallCoroutine(Vector3 deformForce)
    {
        float timeElapsed = 0f;
        while (timeElapsed < bounceDeformCurve.keys[bounceDeformCurve.keys.Length-1].time)
        {
            deformTransform.localScale = Vector3.one - deformForce * bounceDeformCurve.Evaluate(timeElapsed);
            deformTransform.localPosition = -deformForce * bounceDeformCurve.Evaluate(timeElapsed);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        deformTransform.localScale = Vector3.one;

    }

    public void UseGrapple()
    {
        if (blockGameInputs) return;
    }

    public void ReleaseGrapple()
    {
        if (blockGameInputs) return;
    }

    public void Boost()
    {
        if (blockGameInputs) return;
    }

    //Start the game
    public void Spawn()
    {
        Checkpoint spawn = CheckpointManager.instance.spawnPoint;
        CheckpointManager.instance.currentCheckpoint = spawn;
        transform.SetPositionAndRotation(spawn.respawnAnchor.transform.position, spawn.respawnAnchor.transform.rotation);

        TimerManager.instance.StartTimer();

        onSpawn.Invoke();
    }

    public void TryRespawn(Checkpoint checkpoint, bool keepVel)
    {
        if (blockGameInputs) return;
        Respawn(checkpoint, keepVel);
    }

    public void Respawn(Checkpoint checkpoint, bool keepVel)
    {
        if (checkpoint == CheckpointManager.instance.spawnPoint) //Reset game
        {
            CheckpointManager.instance.currentCheckpoint = checkpoint;
            TimerManager.instance.ResetTimer();
        }

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

    public bool IsDying()
    {
        return isDying != null;
    }

    public void Die()
    {
        if (IsDying()) return;
        isDying = StartCoroutine(PlayDeathAnimation());
    }

    private IEnumerator PlayDeathAnimation()
    {
        inputs.Disable();
        onDeathStart.Invoke();

        yield return new WaitForSeconds(1.0f); //TODO : play animation

        Respawn(CheckpointManager.instance.currentCheckpoint, false);
        isDying = null;

        onDeathEnd.Invoke();
        inputs.Enable();
    }
}
