using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

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
    [Header("Player Visuals")]
    [SerializeField] AnimationCurve bounceDeformCurve;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] AnimationCurve trailLengthPerSpeedCurve;
    [SerializeField] float trailCurveLog;
    [SerializeField] float trailStartOffset;
    [SerializeField] ParticleSystem killParticles;
    [SerializeField] float trailLengthLerp;
    [Header("Ground Visuals")]
    [SerializeField] Material groundMaterial;
    [SerializeField] float switchTextureDuration = 0.5f;
    [SerializeField] public PostProcessingController postProcessController;
    [Header("Grappling Hook")]
    [HideInInspector] public List<GrippableObject> grippableColliders;
    [SerializeField] LineRenderer hookRenderer;
    [SerializeField] float hookTime;
    [SerializeField] Material outlineMat;
    [SerializeField] RectTransform hookCursor;

    public Rigidbody rb;

    Coroutine isDying = null;
    Coroutine deformCor;
    Coroutine visualHookCor;

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
    public UnityEvent onLevelReset;

    public bool isGrounded;
    bool canJump;
    bool isBouncing;
    bool isGripped;
    GrippableObject grippableObject;
    Vector3 grippableObjectPoint;
    ConfigurableJoint hookJoint;
    Plane[] planes;
    public bool blockGameInputs = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        rb = GetComponent<Rigidbody>();
        hookJoint = GetComponent<ConfigurableJoint>();

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
        grappleAction.canceled += (_) => ReleaseGrapple(false);
        boostAction.performed += (_) => Boost();
        resetCheckpointAction.performed += (_) => TryRespawn(CheckpointManager.instance.currentCheckpoint, CheckpointManager.instance.currentCheckpoint.keepVelocity);
        resetLevelAction.performed += (_) => TryReset();
        pauseMenuAction.performed += (_) => OpenPauseMenu();

        inputs.Enable();
    }

    private void OnDestroy()
    {
        inputs.Dispose();
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

        if (!isGripped)
            CheckForGrippableObject(out grippableObject, out grippableObjectPoint);

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 forwardVec = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up);
        Vector3 rightVec = Vector3.ProjectOnPlane(playerCamera.transform.right, Vector3.up);
        Vector3 realMove = Time.deltaTime * moveSpeed * (rightVec * moveInput.x + forwardVec * moveInput.y);

        Vector2 velAxis = Vector2.Perpendicular(new Vector2(rb.velocity.x, rb.velocity.z)).normalized;

        visualSphere.Rotate(new Vector3(-velAxis.x, 0, -velAxis.y), rb.velocity.magnitude * Time.deltaTime * 50f, Space.World);
        hookCursor.gameObject.SetActive(!hookRenderer.enabled && grippableObject != null);
        if (grippableObject != null && !isGripped)
            hookCursor.anchoredPosition = RectTransformUtility.WorldToScreenPoint(playerCamera, grippableObjectPoint);
        if (hookRenderer.enabled && visualHookCor == null && isGripped)
            hookRenderer.SetPosition(1, grippableObjectPoint - transform.position);

        trailRenderer.time = Mathf.Lerp(trailRenderer.time, Mathf.Max(trailLengthPerSpeedCurve.Evaluate(Mathf.Log(rb.velocity.magnitude, trailCurveLog)) - trailStartOffset, 0), trailLengthLerp);
        trailRenderer.enabled = (trailLengthPerSpeedCurve.Evaluate(Mathf.Log(rb.velocity.magnitude, trailCurveLog)) - trailStartOffset) > 0;

        if (!blockGameInputs) rb.AddForce(realMove, ForceMode.Force);
    }

    #region Jump

    public void Jump()
    {
        if (blockGameInputs || !canJump || !isGrounded) return;

        canJump = false;
        AudioManager.PlaySound("Jump", UnityEngine.Random.Range(0.8f, 1.2f));
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

    IEnumerator DeformBallCoroutine(Vector3 deformForce)
    {
        float timeElapsed = 0f;
        while (timeElapsed < bounceDeformCurve.keys[bounceDeformCurve.keys.Length - 1].time)
        {
            deformTransform.localScale = Vector3.one - deformForce * bounceDeformCurve.Evaluate(timeElapsed);
            deformTransform.localPosition = -deformForce * bounceDeformCurve.Evaluate(timeElapsed);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        deformTransform.localScale = Vector3.one;
    }

    #endregion

    #region Switch bounce and slippery

    public void SwitchBounce(bool nowBouncing)
    {
        if (blockGameInputs || sphereCollider == null) return;

        sphereCollider.material = nowBouncing ? bounceMat : nofrictionMat;
        isBouncing = nowBouncing;

        AudioManager.PlaySound(nowBouncing ? "SetBouncy" : "SetNofriction");
        StartCoroutine(SwitchTextureCoroutine(!nowBouncing));
    }

    IEnumerator SwitchTextureCoroutine(bool bounceToSlippery)
    {
        float t = 0;
        while (t < switchTextureDuration)
        {
            yield return null;
            t += Time.deltaTime;
            groundMaterial.SetFloat("_BounceToSlipperyCoeff", (bounceToSlippery ? t/switchTextureDuration : (1 - t / switchTextureDuration) ));
        }
        groundMaterial.SetFloat("_BounceToSlipperyCoeff", (bounceToSlippery ? 1 : 0));

    }
    #endregion


    void OnCollisionEnter(Collision collision)
    {
        if (isBouncing)
        {
            if (deformCor != null)
                StopCoroutine(deformCor);
            AudioManager.PlaySound("Bounce", UnityEngine.Random.Range(0.8f, 1.2f));
            deformCor = StartCoroutine(DeformBallCoroutine(collision.impulse.normalized));
        }
    }

    #region Grapple
    public void UseGrapple()
    {
        if (grippableObject == null || blockGameInputs) return;
        isGripped = true;
        hookJoint.connectedBody = grippableObject.GetComponent<Rigidbody>();
        Vector3 anchor = grippableObjectPoint - grippableObject.transform.position;
        hookJoint.connectedAnchor = //Vector3.zero;
            new Vector3(anchor.x / grippableObject.transform.localScale.x, anchor.y / grippableObject.transform.localScale.y, anchor.z / grippableObject.transform.localScale.z);
        hookJoint.linearLimit = new SoftJointLimit{
            limit = Vector3.Distance(transform.position, grippableObjectPoint),
            bounciness = 0,
            contactDistance = 0
            };
        hookJoint.xMotion = ConfigurableJointMotion.Limited;
        hookJoint.yMotion = ConfigurableJointMotion.Limited;
        hookJoint.zMotion = ConfigurableJointMotion.Limited;
        if (visualHookCor != null)
            StopCoroutine(visualHookCor);
        AudioManager.PlaySound("UseGrapple");
        visualHookCor = StartCoroutine(HookCoroutine(true, grippableObjectPoint));
    }

    public void ReleaseGrapple(bool instant)
    {
        if (blockGameInputs) return;

        isGripped = false;
        hookJoint.connectedBody = null;
        hookJoint.xMotion = ConfigurableJointMotion.Free;
        hookJoint.yMotion = ConfigurableJointMotion.Free;
        hookJoint.zMotion = ConfigurableJointMotion.Free;

        if (!instant && hookRenderer.enabled)
        {
            if (visualHookCor != null)
                StopCoroutine(visualHookCor);
            AudioManager.PlaySound("ReleaseGrapple");
            visualHookCor = StartCoroutine(HookCoroutine(false, grippableObjectPoint));
        }
        else 
        {
            hookRenderer.enabled = false;
        }
    }

    IEnumerator HookCoroutine(bool hooking, Vector3 point)
    {
        if (hooking)
            hookRenderer.enabled = true;
        float timeElapsed = 0;
        if (hooking)
            timeElapsed = hookRenderer.GetPosition(1).magnitude;
        else
            timeElapsed = Vector3.Distance(hookRenderer.GetPosition(1), point - transform.position);

        timeElapsed = timeElapsed / Vector3.Distance(point, transform.position) / hookTime;
        while (timeElapsed < hookTime)
        {
            hookRenderer.SetPosition(1, Vector3.Lerp(Vector3.zero, point - transform.position, hooking ? timeElapsed/hookTime : 1 - timeElapsed/hookTime));
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        hookRenderer.SetPosition(1, hooking ? point - transform.position : Vector3.zero);
        if (!hooking)
            hookRenderer.enabled = false;
        visualHookCor = null;
    }



    bool CheckForGrippableObject(out GrippableObject gripObject, out Vector3 gripPoint)
    {
        planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        GrippableObject oldGrip = grippableObject;
        float minDist = float.MaxValue;
        gripObject = null;
        gripPoint = Vector3.zero;
        foreach (GrippableObject grip in grippableColliders)
        {
            Vector3 preferredPoint = grip.GetPreferredGrabPoint(transform, playerCamera.transform);
            if (GeometryUtility.TestPlanesAABB(planes, grip.bounds) && grip.IsGrippable(transform, preferredPoint))
            {
                float d = Vector3.Distance(transform.position, preferredPoint);
                if (minDist > d)
                {
                    minDist = d;
                    gripObject = grip;
                    gripPoint = preferredPoint;
                }
            }
        }
        if (oldGrip != gripObject)
        {
            if (oldGrip != null)
                RemoveOutlineMat(oldGrip);
            if (gripObject != null)
                AddOutlineMat(gripObject);
        }
        return minDist != float.MaxValue;
    }

    #endregion

    public void Boost()
    {

    }

    #region line renderer

    void AddOutlineMat(GrippableObject grip)
    {
        List<Material> mats = new List<Material>();
        grip.rend.GetMaterials(mats);
        mats.Add(outlineMat);
        grip.rend.SetMaterials(mats);
    }

    void RemoveOutlineMat(GrippableObject grip)
    {
        List<Material> mats = new List<Material>();
        grip.rend.GetMaterials(mats);
        mats.RemoveAt(mats.Count-1);
        grip.rend.SetMaterials(mats);
    }
    #endregion

    #region Spawn et restart
    //Start the game
    public void Spawn()
    {
        Checkpoint spawn = CheckpointManager.instance.spawnPoint;
        CheckpointManager.instance.currentCheckpoint = spawn;

        transform.position = spawn.respawnAnchor.transform.position;
        spawn.cameraPosition = spawn.cameraRespawnAnchor.transform.position;
        spawn.cameraRotation = spawn.cameraRespawnAnchor.transform.rotation;
        rb.velocity = Vector3.zero;

        PlayerCamera.Instance.SetCamTransform(spawn.cameraRespawnAnchor.transform.position, spawn.cameraRespawnAnchor.transform.rotation);

        TimerManager.instance.StartTimer();

        onSpawn.Invoke();
    }

    public void TryRespawn(Checkpoint checkpoint, bool keepVel)
    {
        if (blockGameInputs) return;
        Respawn(checkpoint, keepVel);
    }

    public void TryReset()
    {
        if (blockGameInputs) return;
        LevelManager.instance.ResetLevel(this);
    }

    public void Respawn(Checkpoint checkpoint, bool keepVel)
    {
        ReleaseGrapple(true);

        if (checkpoint == CheckpointManager.instance.spawnPoint) //Reset game
        {
            CheckpointManager.instance.currentCheckpoint = checkpoint;
            TimerManager.instance.ResetTimer();
            onLevelReset.Invoke();
        }

        if (keepVel)
        {
            transform.position = checkpoint.position;
            rb.velocity = checkpoint.velocity;
            PlayerCamera.Instance.SetCamTransform(checkpoint.cameraPosition, checkpoint.cameraRotation);
            //Camera.main.transform.SetPositionAndRotation(checkpoint.cameraPosition, checkpoint.cameraRotation);
        }
        else
        {
            transform.position = checkpoint.respawnAnchor.transform.position;
            rb.velocity = Vector3.zero;
            PlayerCamera.Instance.SetCamTransform(checkpoint.cameraRespawnAnchor.transform.position, checkpoint.cameraRespawnAnchor.transform.rotation);
            Camera.main.transform.SetPositionAndRotation(checkpoint.cameraRespawnAnchor.transform.position, checkpoint.cameraRespawnAnchor.transform.rotation);
        }

        onRespawn.Invoke();
    }

    #endregion

    public void OpenPauseMenu()
    {
        if (blockGameInputs) return;
        GameUI.instance.TogglePause();
    }

    #region Death

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

        AudioManager.PlaySound("Death");

        killParticles.Play();
        
        visualSphere.gameObject.SetActive(false);
        rb.isKinematic = true;

        yield return new WaitForSeconds(0.6f); //TODO : play animation
        GameUI.instance.ShowDeathScreen(0.2f, 0.3f, 0.2f);
        yield return new WaitForSeconds(0.3f);

        visualSphere.gameObject.SetActive(true);
        rb.isKinematic = false;

        Respawn(CheckpointManager.instance.currentCheckpoint, CheckpointManager.instance.currentCheckpoint.keepVelocity);
        isDying = null;

        onDeathEnd.Invoke();
        inputs.Enable();
    }
    #endregion
}
