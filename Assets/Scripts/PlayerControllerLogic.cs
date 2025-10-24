using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerLogic : MonoBehaviour
{
    private Animator animator;
    private PlayerController controls;
    private Vector2 moveInput;
    private bool isJumping;
    private bool isShooting;
    private int currentHealth = 100;

    [Header("References")]
    public Transform cameraTransform; // <-- за посоката на движение спрямо камерата

    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 720f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controls = new PlayerController();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => isJumping = true;
        controls.Player.Jump.canceled += ctx => isJumping = false;

        controls.Player.Shoot.performed += ctx => isShooting = true;
        controls.Player.Shoot.canceled += ctx => isShooting = false;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Update()
    {
        HandleMovement();
        HandleAnimations();
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        if (move.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothTurnVelocity, rotationSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

          
            transform.position += moveDir.normalized * speed * Time.deltaTime;
        }
    }

    private float smoothTurnVelocity;

    private void HandleAnimations()
    {
        float animationSpeed = moveInput.magnitude;
        animator.SetFloat("Speed", animationSpeed);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsShooting", isShooting);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player'a {damage} hasar geldi! Kalan can: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Player öldü!");
            // Öldüğünde yapılacaklar
        }
    }
}
