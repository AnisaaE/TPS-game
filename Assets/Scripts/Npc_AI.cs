using UnityEngine;
using UnityEngine.AI;

public class Npc_AI : MonoBehaviour
{
    // Mevcut deðiþkenler
    public float walkRadius = 20f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    private float nextDestinationTime;

    // YENÝ EKLENEN DEÐÝÞKENLER
    [Header("Animasyon ve Durum Kontrolü")]
    private Animator animator;
    public float initialIdleDuration = 10f; // Baþlangýç bekleme süresi
    private float initialIdleTimer = 0f;
    private bool isInitialIdleDone = false;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Animator bileþenini al

        if (agent == null || animator == null)
        {
            Debug.LogError("NavMeshAgent veya Animator bileþeni bulunamadý!");
            enabled = false;
            return;
        }

        // Baþlangýçta 10 saniye beklemesi için NPC'yi durdur
        agent.isStopped = true;
    }

    void Update()
    {
        // 1. BAÞLANGIÇTA IDLE SÜRESÝ KONTROLÜ
        if (!isInitialIdleDone)
        {
            // IDLE animasyonu için Speed parametresini 0 yap
            animator.SetFloat("Speed", 0f);

            initialIdleTimer += Time.deltaTime;

            if (initialIdleTimer >= initialIdleDuration)
            {
                isInitialIdleDone = true;
                agent.isStopped = false; // NPC'yi hareket ettirmeye baþla
                SetNewRandomDestination(); // Ýlk devriye hedefini belirle
                Debug.Log("10 saniyelik bekleme bitti, devriye baþlýyor.");
            }
            return; // 10 saniye dolana kadar Update'in geri kalanýný çalýþtýrma
        }

        // 2. DEVRIYE (PATROL) MANTIÐI

        // Animasyon kontrolü: Agent'ýn gerçek hýzýný al ve Animator'daki Speed parametresine set et.
        // agent.velocity.magnitude -> Ajanýn gerçek hareket hýzý
        // agent.speed -> Ajanýn maksimum ayarlanmýþ hýzý (hýzý normalize etmek için kullandýk)
        float normalizedSpeed = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", normalizedSpeed);


        // Hedefe ulaþtýysa ve bekleme süresi dolduysa
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // Yeni bir hedef belirlemeden önce bekleme süresi doldu mu?
            if (Time.time >= nextDestinationTime)
            {
                // SetNewRandomDestination() fonksiyonu içinde zaten agent.SetDestination çaðrýlýyor.
                SetNewRandomDestination();
            }
        }
    }

    // ... SetNewRandomDestination ve GetRandomPoint fonksiyonlarý ayný kalýr.
    private void SetNewRandomDestination()
    {
        // ... (Eski kodunuz ayný kalacak) ...
        Vector3 randomPoint = GetRandomPoint(transform.position, walkRadius);
        agent.SetDestination(randomPoint);
        nextDestinationTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
    }

    private Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        // ... (Eski kodunuz ayný kalacak) ...
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return center;
    }
}