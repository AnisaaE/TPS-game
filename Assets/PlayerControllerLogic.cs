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
    private Vector3 aimPoint;
   

    [Header("References")]
    public CinemachineCamera vCamNormal;   
    public CinemachineCamera vCamAim;      
    public Transform cameraTransform;      
    public GameObject crosshairUI;         
    public Transform shootOrigin;         
    public LayerMask enemyLayer;           

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

    [Header("Audio")]
    public AudioSource audioSource;        
    public AudioClip footstepSound;       
    public AudioClip gunshotSound;         

    public float footstepInterval = 0.4f;  


    private float yaw;
    private float pitch;
    private float smoothTurnVelocity;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        controls = new PlayerController();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => isJumping = true;
        controls.Player.Jump.canceled += ctx => isJumping = false;

        controls.Player.Aim.performed += ctx =>
        {
            isAiming = !isAiming;
            if (crosshairUI != null)
                crosshairUI.SetActive(isAiming);
        };

        controls.Player.Shoot.performed += ctx => isShooting = true;
        controls.Player.Shoot.canceled += ctx => isShooting = false;

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
     
        if (isShooting && isAiming)
            Shoot();
    }

    private void HandleCamera()
    {
        if (vCamNormal == null || vCamAim == null) return;
        vCamNormal.Priority = isAiming ? 0 : 10;
        vCamAim.Priority = isAiming ? 10 : 0;
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothTurnVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            transform.position += moveDir.normalized * speed * Time.deltaTime;
            animator.SetFloat("Speed", 1f);

            if (!audioSource.isPlaying && footstepSound != null)
            {
                audioSource.clip = footstepSound;
                audioSource.loop = true;  
                audioSource.Play();
            }
        }
        else
        {
            animator.SetFloat("Speed", 0f);
            if (audioSource.isPlaying)
                audioSource.Stop();  
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

        Ray ray = new Ray(shootOrigin.position, shootOrigin.forward);
        RaycastHit hit;

        Debug.DrawRay(shootOrigin.position, shootOrigin.forward * shootRange, Color.red, 1f);
        if (gunshotSound != null && audioSource != null)
            audioSource.PlayOneShot(gunshotSound);

        if (Physics.Raycast(ray, out hit, shootRange, enemyLayer))
        {
            Debug.Log("NPC vuruldu: " + hit.collider.name);

            Npc_AI npc = hit.collider.GetComponentInParent<Npc_AI>();
            if (npc != null)
            {
                npc.TakeDamage(damage);
                Debug.Log("Damage uygulandı.");
            }
        }
        else
        {
            Debug.Log("Hiçbir şey vurulmadı.");
        }
    }
    private void AlignShootOriginWithCamera()
    {
        if (shootOrigin == null || cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, shootRange))
        {
            aimPoint = hit.point;
        }
            
        else
        {
            aimPoint = ray.origin + ray.direction * shootRange;

        }

        Vector3 aimDir = (aimPoint - shootOrigin.position).normalized;
        shootOrigin.forward = Vector3.Lerp(shootOrigin.forward, aimDir, Time.deltaTime * 20f);
    }

    public void ReceiveDamage(int amount)
    {
        if (playerHealth != null)
            playerHealth.TakeDamage(amount);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void LateUpdate()
    {
        AlignShootOriginWithCamera();
    }
    public void StopAllAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

}