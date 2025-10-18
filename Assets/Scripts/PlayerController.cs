using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerLogic : MonoBehaviour
{
    private Animator animator;
    private PlayerController controls;   // Генерирания клас от Input Actions
    private Vector2 moveInput;
    private bool isJumping;
    private bool isShooting;

    [Header("Movement Settings")]
    public float speed = 5f; // Колко бързо се движи играчът

    private void Awake()
    {
        // Взимаме Animator компонента на Player
        animator = GetComponent<Animator>();

        // Създаваме инстанция на Input Actions (генерирания клас)
        controls = new PlayerController();

        // ---------- MOVE ----------
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // ---------- JUMP ----------
        controls.Player.Jump.performed += ctx => isJumping = true;
        controls.Player.Jump.canceled += ctx => isJumping = false;

        // ---------- SHOOT ----------
        controls.Player.Shoot.performed += ctx => isShooting = true;
        controls.Player.Shoot.canceled += ctx => isShooting = false;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Update()
    {
        // ---------- Movement ----------
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        // Движение
        transform.Translate(move * speed * Time.deltaTime, Space.Self);

        // Плавно завъртане към посоката на движение
        if (move != Vector3.zero)
        {
            // Целева ротация
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);

            // Плавно завъртане (10f е скоростта, можеш да я промениш)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
        }

        // ---------- Анимации ----------
        float animationSpeed = moveInput.magnitude;
        animator.SetFloat("Speed", animationSpeed);

        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsShooting", isShooting);
    }


}
