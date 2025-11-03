using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // Unity 6 için doğru namespace
using UnityEngine.UI;

public class PlayerControllerLogic : MonoBehaviour
{
    private Animator animator;
    private PlayerController controls;
    private Vector2 moveInput;
    private bool isJumping;
    private bool isAiming;
    private bool isShooting;

    [Header("References")]
    public CinemachineCamera vCamNormal;   // Normal kamera (VCam_Normal)
    public CinemachineCamera vCamAim;      // Aim kamera (VCam_Aim)
    public Transform cameraTransform;      // MainCamera
    public GameObject crosshairUI;         // Crosshair objesi (Canvas içinde)
    public Transform shootOrigin;          // Silahın ucu
    public LayerMask enemyLayer;           // NPC layer

    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSmoothTime = 0.1f;
    private float rotationSmoothVelocity;

    [Header("Shooting Settings")]
    public float shootRange = 100f;
    public int damage = 20;

    // Diğer bileşen referansı
    private PlayerHealth playerHealth;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>(); // PlayerHealth scriptine erişim
        controls = new PlayerController();

        // --- Hareket ---
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // --- Zıplama ---
        controls.Player.Jump.performed += ctx => isJumping = true;
        controls.Player.Jump.canceled += ctx => isJumping = false;

        // --- Sağ tıkla aim ---
        controls.Player.Aim.performed += ctx =>
        {
            isAiming = !isAiming;
            if (crosshairUI != null)
                crosshairUI.SetActive(isAiming);
        };

        // --- Sol tıkla ateş ---
        controls.Player.Shoot.performed += ctx => isShooting = true;
        controls.Player.Shoot.canceled += ctx => isShooting = false;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleAnimation();

        // Ateş işlemi
        if (isShooting && isAiming)
            Shoot();
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSmoothVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            transform.position += moveDir.normalized * speed * Time.deltaTime;

            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    private void HandleCamera()
    {
        if (vCamNormal == null || vCamAim == null) return;
        vCamNormal.Priority = isAiming ? 0 : 10;
        vCamAim.Priority = isAiming ? 10 : 0;
    }

    private void HandleAnimation()
    {
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsAiming", isAiming);
        animator.SetBool("IsShooting", isShooting);
    }

    private void Shoot()
    {
        Ray ray = new Ray(shootOrigin.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootRange, enemyLayer))
        {
            Debug.Log("NPC vuruldu: " + hit.collider.name);

            // NPC’ye hasar ver
            Npc_AI npc = hit.collider.GetComponent<Npc_AI>();
            if (npc != null)
            {
                npc.TakeDamage(damage);
            }
        }
    }

    // NPC seni vurduğunda PlayerHealth üzerinden çağrılacak:
    public void ReceiveDamage(int amount)
    {
        if (playerHealth != null)
            playerHealth.TakeDamage(amount);
    }
}