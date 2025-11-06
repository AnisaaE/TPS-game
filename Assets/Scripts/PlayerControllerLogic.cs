using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.UI;

public class PlayerControllerLogic : MonoBehaviour
{
    private Animator animator;
    private PlayerController controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
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
    private Transform aimCamTransform;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSmoothTime = 0.12f;

    [Header("Mouse Look Settings")]
    public float cameraSensitivityX = 250f;
    public float cameraSensitivityY = 120f;
    public float cameraPitchLimit = 80f;
    public float sensitivityMultiplier = 0.8f;

    [Header("Shooting Settings")]
    public float shootRange = 100f;
    public int damage = 20;

    private float yaw;
    private float pitch;
    private float smoothTurnVelocity;

    // Diğer bileşen referansı
    private PlayerHealth playerHealth;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        controls = new PlayerController();
        aimCamTransform = vCamAim.transform;

        // --- Hareket ---
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // --- Mouse Look ---
        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

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

        // Fare imlecini gizle ve kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Update()
    {
        HandleCamera();
        HandleMovement();
        HandleAnimation();

        // Ateş işlemi
        if (isShooting && isAiming)
            Shoot();
    }

    private void HandleCamera()
    {
        // Mouse look - yaw ve pitch güncelleme
        yaw += lookInput.x * cameraSensitivityX * Time.deltaTime * sensitivityMultiplier * 0.2f;
        pitch -= lookInput.y * cameraSensitivityY * Time.deltaTime * sensitivityMultiplier * 0.2f;
        pitch = Mathf.Clamp(pitch, -cameraPitchLimit, cameraPitchLimit);

        // Ana kameraya dönüş uygula
        if (cameraTransform != null)
        {
            cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        // Kamera geçişleri
        if (vCamNormal == null || vCamAim == null) return;
        vCamNormal.Priority = isAiming ? 0 : 10;
        vCamAim.Priority = isAiming ? 10 : 0;
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Kameranın yönüne göre hareket açısı
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Yumuşak dönüş
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothTurnVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            // Hareket vektörü
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            transform.position += moveDir.normalized * speed * Time.deltaTime;

            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    private void HandleAnimation()
    {
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsAiming", isAiming);
        animator.SetBool("IsShooting", isShooting);
    }

    private void Shoot()
    {
        Debug.Log("Shoot fonksiyonu çalıştı.");
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * shootRange, Color.red, 5f);
        //Debug.DrawRay(ray.origin, ray.direction * 200f, Color.red, 10f);

        if (Physics.Raycast(ray, out hit, shootRange, enemyLayer))
        {
            Debug.Log("NPC vuruldu: " + hit.collider.name);

            Npc_AI npc = hit.collider.GetComponentInParent<Npc_AI>();
            if (npc != null)
            {
                npc.TakeDamage(damage);
                Debug.Log("Damage uygulandı.");
            }
            else
            {
                Debug.LogWarning("Çarpılan objede Npc_AI scripti bulunamadı!");
            }
        }
        else
        {
            Debug.Log("Hiçbir şey vurulmadı.");
        }
    }



    public void ReceiveDamage(int amount)
    {
        if (playerHealth != null)
            playerHealth.TakeDamage(amount);
    }

    // Debug için - ESC ile fareyi serbest bırakma
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}