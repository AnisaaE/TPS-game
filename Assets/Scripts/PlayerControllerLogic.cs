using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerLogic : MonoBehaviour, PlayerController.IPlayerActions
{
    // Inspector'da atayacağınız bileşenler
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform followCamera; // Kamera Transform'u (dönüş için)

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 500f;

    // A. MADDESİNDEKİ YENİ KODLAR BURADA: Jump Settings
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f; // Zıplama gücü
    [SerializeField] private float groundCheckDistance = 0.2f; // Zemin kontrol mesafesi
    [SerializeField] private LayerMask groundLayer; // Zemin olarak kabul edilecek katman

    private bool isGrounded = true; // Karakterin zeminde olup olmadığını tutar
    // Tahmini Animator parametresi (Sizin animasyon sisteminize göre "Jump" veya "IsJumping" olabilir)
    private readonly int isJumpingHash = Animator.StringToHash("IsJumping");
    // ^^^ Kendi animasyon parametre adınıza göre değiştirin! ^^^

    // Silah objesi
    [Header("Weapon Settings")]
    [SerializeField] private GameObject ak74Weapon;

    // Input System değişkenleri
    private PlayerController inputActions;
    private Vector2 moveInput;

    // Animator Parametre Adları
    private readonly int moveSpeedHash = Animator.StringToHash("Speed");
    private readonly int isShootingHash = Animator.StringToHash("isShooting");

    // Başlangıç Ayarları
    void Awake()
    {
        // Gerekli bileşenleri al ve kontrol et
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();

        // Rigidbody ayarları
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        // Input Actions nesnesini oluştur ve callback'leri ata
        inputActions = new PlayerController();
        inputActions.Player.SetCallbacks(this);

        if (followCamera == null && Camera.main != null)
        {
            followCamera = Camera.main.transform;
        }

        if (ak74Weapon != null)
        {
            ak74Weapon.SetActive(true);
        }
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    // --- Input System Callback Metotları ---

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // YENİ ZIPLAMA MANTIĞI BURADA BAŞLIYOR
    public void OnJump(InputAction.CallbackContext context)
    {
        // Zıplama tuşuna basıldığında ve karakter zemindeyse
        if (context.performed && isGrounded)
        {
            HandleJump();
        }
    }
    // YENİ ZIPLAMA MANTIĞI BURADA BİTİYOR

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetBool(isShootingHash, true);
            PerformShoot();
        }

        if (context.canceled)
        {
            animator.SetBool(isShootingHash, false);
        }
    }

    // --- Oyun Mantığı ---

    void Update()
    {
        HandleRotation();
    }

    void FixedUpdate()
    {
        CheckIfGrounded(); // Zemin kontrolü (Çok önemli!)
        HandleMovement();
    }

    // YENİ ZIPLAMA İŞLEM METODU
    private void HandleJump()
    {
        if (rb == null) return;

        // 1. Zıplama Kuvveti Uygulama: Rigidbody'ye dikey hız ekle
        // Mevcut dikey hızı sıfırla ki zıplama gücü her zaman aynı olsun
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        // Anlık, güçlü bir zıplama kuvveti uygula
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // 2. Animasyonu Tetikleme
        animator.SetBool(isJumpingHash, true);
        isGrounded = false; // Anlık olarak zeminde değiliz bayrağını kaldır
    }

    // YENİ ZEMİN KONTROL METODU
    private void CheckIfGrounded()
    {
        if (rb == null) return;

        // Karakterin altından bir ışın (Raycast) yolla
        RaycastHit hit;
        // Not: GroundCheckDistance mutlaka 0'dan büyük olmalı!
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            // Zemindeyiz
            isGrounded = true;
            // Animasyon kontrolü: Yere değdiğinde zıplama animasyonunu kapat
            animator.SetBool(isJumpingHash, false);
        }
        else
        {
            // Havadayız
            isGrounded = false;
        }
    }


    private void HandleMovement()
    {
        if (rb == null) return;

        if (moveInput.magnitude < 0.1f)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            animator.SetFloat(moveSpeedHash, 0f);
            return;
        }

        Vector3 cameraForward = followCamera.forward;
        Vector3 cameraRight = followCamera.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveVector = cameraForward * moveInput.y + cameraRight * moveInput.x;

        Vector3 newVelocity = moveVector.normalized * moveSpeed;
        rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);

        animator.SetFloat(moveSpeedHash, moveInput.magnitude);
    }

    private void HandleRotation()
    {
        // Karakterin, kameranın baktığı yöne doğru dönmesini sağlar
        if (moveInput.magnitude >= 0.1f && followCamera != null)
        {
            Vector3 targetDirection = followCamera.forward * moveInput.y + followCamera.right * moveInput.x;
            targetDirection.y = 0;

            if (targetDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    private void PerformShoot()
    {
        Debug.Log("Fire! Sol tık ile ateş edildi.");
    }
}