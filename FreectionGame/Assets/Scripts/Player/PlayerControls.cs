using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    public static PlayerControls Instance;

    [SerializeField] float moveSpeed;
    [SerializeField] PhysicMaterial bounceMat;
    [SerializeField] PhysicMaterial nofrictionMat;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float jumpDelay;
    [SerializeField] float jumpStrength;


    Rigidbody rb;
    [SerializeField] SphereCollider sphereCollider;

    Inputs inputs;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction bounceAction;
    InputAction grappleAction;
    InputAction boostAction;
    InputAction resetCheckpointAction;
    InputAction resetLevelAction;
    InputAction pauseMenuAction;

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
        resetCheckpointAction.performed += (_) => ResetPlayer(false, false);
        resetLevelAction.performed += (_) => ResetPlayer(true, false);
        pauseMenuAction.performed += (_) => OpenPauseMenu();

        inputs.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        Vector3 realMove = Time.deltaTime * moveSpeed * (transform.rotation * new Vector3(moveInput.x, 0, moveInput.y));

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

    public void ResetPlayer(bool firstCheckpoint, bool keepVel)
    {

    }

    public void OpenPauseMenu()
    {

    }

}
