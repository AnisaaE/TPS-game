using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerControllerLogic : MonoBehaviour
{
    private Animator animator;
    private PlayerController controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isJumping;
    private bool isShooting;

    [Header("References")]
    public Transform cameraTransform;   // MainCamera
    public Transform camTarget;         // camTarget (точка за следене)

    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSmoothTime = 0.12f;
    public float cameraSensitivityX = 250f;
    public float cameraSensitivityY = 120f;
    public float cameraPitchLimit = 80f;
    public float sensitivityMultiplier = 0.8f;

    private float yaw;
    private float pitch;
    private float smoothTurnVelocity;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controls = new PlayerController();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => isJumping = true;
        controls.Player.Jump.canceled += ctx => isJumping = false;

        controls.Player.Shoot.performed += ctx => isShooting = true;
        controls.Player.Shoot.canceled += ctx => isShooting = false;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void LateUpdate()
    {
        HandleCamera();
        HandleMovement();
        HandleAnimations();
    }

    private void HandleCamera()
    {
        // Обновяване на yaw/pitch от мишката
        yaw += lookInput.x * cameraSensitivityX * Time.deltaTime* sensitivityMultiplier;
        pitch -= lookInput.y * cameraSensitivityY * Time.deltaTime * sensitivityMultiplier;
        pitch = Mathf.Clamp(pitch, -cameraPitchLimit, cameraPitchLimit);

        // Прилагаме въртенето върху camTarget
        camTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        if (move.magnitude >= 0.01f)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + camTarget.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothTurnVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            transform.position += moveDir.normalized * speed * Time.deltaTime;
        }
    }

    private void HandleAnimations()
    {
        float animationSpeed = moveInput.magnitude;
        animator.SetFloat("Speed", animationSpeed);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsShooting", isShooting);
    }
}
