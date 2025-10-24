using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Npc_AI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public int maxHealth = 100; //npc toplam canı
    private int currentHealth;

    [Header("Mesafeler")]
    public float chaseDistance = 8f;
    public float attackDistance = 3f;
    public float patrolRadius = 15f;
    public float patrolWaitTime = 3f;
    public float attackRate = 1f; // 1 saniyede bir ateş et

    [Header("Animasyon")]
    public Animator animator;

    private Vector3 patrolTarget;
    private bool isPatrolling = true;
    private bool isAttacking = false; // Ateş etme durumunu kontrol eder
    private bool isDead = false;// Npc öldü mü kontrolü

    void Start()
    {
        currentHealth = maxHealth;
        GoToNextPatrolPoint();
        StartCoroutine(PatrolRoutine());
    }
    void Update()
    {
        if (isDead) return; // ☠️ Öldüyse hiçbir şey yapma
        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 direction = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, direction);

        if (angle < 120f)
        {
            if (distance <= attackDistance)
            {
                AttackPlayer();
            }
            else if (distance <= chaseDistance)
            {
                StopCoroutine("AttackLoop");
                isAttacking = false;
                animator.SetBool("isShooting", false); // 🔥 Ekledik
                ChasePlayer();
            }
            else
            {
                StopCoroutine("AttackLoop");
                isAttacking = false;
                animator.SetBool("isShooting", false); // 🔥 Ekledik
                Patrol();
            }
        }
        else
        {
            StopCoroutine("AttackLoop");
            isAttacking = false;
            animator.SetBool("isShooting", false); // 🔥 Ekledik
            Patrol();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        Debug.Log($"NPC'ye {damage} hasar geldi! Kalan can: {currentHealth}");

        if(currentHealth <= 0)
        {
            Die();

        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f && isPatrolling)
        {
            StartCoroutine(PatrolRoutine());
        }

        agent.isStopped = false;

        if (animator)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        if (animator)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);
        }
    }

    
    void AttackPlayer()
    {
        agent.isStopped = true;
        transform.LookAt(player);

        if (!isAttacking)
        {
            isAttacking = true;
            StartCoroutine("AttackLoop");
        }

        if (animator)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
            // Bu kısım sayesinde koşudan, yürüyüşten veya idleden geçiş yapabilir
        }
    }

    IEnumerator AttackLoop()
    {
        isAttacking = true;
        animator.SetBool("isShooting", true);

        while (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            Debug.Log("NPC ateş ediyor!");

            // Player'a hasar ver
            ///////player.GetComponent<PlayerController>().TakeDamage(10); bunu playercontrollerda eklemen gerekiyorrr!!!!!!!!!!!!

            yield return new WaitForSeconds(attackRate);
        }

        isAttacking = false;
        animator.SetBool("isShooting", false);
    }

    IEnumerator PatrolRoutine()
    {
        isPatrolling = false;
        yield return new WaitForSeconds(patrolWaitTime);
        GoToNextPatrolPoint();
        isPatrolling = true;
    }

    void GoToNextPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1);
        patrolTarget = hit.position;
        agent.SetDestination(patrolTarget);
    }
    
    
    void Die()
    {
        if (isDead) return;
        isDead = true;
        agent.isStopped = true;
        Debug.Log("NPC öldü!");

        // 2 saniye sonra sahneden kaldır
        Destroy(gameObject, 2f);
    }
}
