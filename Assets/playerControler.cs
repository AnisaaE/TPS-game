using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // ��������
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ);
        rb.MovePosition(transform.position + move * speed * Time.deltaTime);

        // ����� �� �������� ������ ���������
        float currentSpeed = move.magnitude;
        animator.SetFloat("Speed", currentSpeed);

        // �������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("IsJumping", true);
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }

        // �������
        if (Input.GetMouseButtonDown(0)) // ��� �����
        {
            animator.SetBool("IsShooting", true);
        }
        else
        {
            animator.SetBool("IsShooting", false);
        }
    }
}

