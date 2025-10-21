using UnityEngine;
using UnityEngine.AI;

// NPC'nin olabilece�i durumlar� tan�ml�yoruz
public enum NPCState
{
    InitialIdle,    // Oyun ba�lad���nda k�sa bekleme
    Patrol,         // Haritada rastgele devriye gezme
    RestIdle,       // Periyodik dinlenme/bekleme durumu
    Chase,          // Yeni: Kovalama
    Shoot           // Yeni: At��/Sald�r�
}

public class Npc_AI : MonoBehaviour
{
    [Header("Hareket Ayarlar�")]
    public float patrolSpeed = 1.5f;
    public float walkRadius = 20f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    private float nextDestinationTime;

    [Header("Durum Kontrol�")]
    public NPCState currentState;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Idle/Dinlenme S�releri")]
    public float initialIdleDuration = 5f;
    private float initialIdleTimer = 0f;
    public float patrolDuration = 15f;
    private float patrolTimer;
    public float restIdleDuration = 3f;
    private float restIdleTimer;

    // YEN� EKLENEN V�ZYON VE KOVALAMA DE���KENLER�
    [Header("G�r�� ve Kovalama Ayarlar�")]
    public float sightRange = 15f;    // NPC'nin g�rebilece�i maksimum mesafe
    public float sightAngle = 90f;    // NPC'nin g�r�� a��s�
    public float chaseSpeed = 4.0f;   // Kovalama durumunda kullan�lacak h�z
    public float shootRange = 5f;     // At�� menzili
    public LayerMask playerMask;      // Oyuncunun bulundu�u Layer

    private Transform playerTarget;   // Bulunan oyuncunun transform'u

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null || animator == null)
        {
            Debug.LogError("NavMeshAgent veya Animator bile�eni bulunamad�!");
            enabled = false;
            return;
        }

        agent.speed = patrolSpeed;
        currentState = NPCState.InitialIdle;
        agent.isStopped = true;
        patrolTimer = patrolDuration;
    }

    void Update()
    {
        // NPC'nin durumu ne olursa olsun (Idle, Patrol, Rest) oyuncuyu alg�lamaya �al��
        CheckForPlayer();

        switch (currentState)
        {
            case NPCState.InitialIdle:
                HandleInitialIdleState();
                break;
            case NPCState.Patrol:
                HandlePatrolState();
                break;
            case NPCState.RestIdle:
                HandleRestIdleState();
                break;
            case NPCState.Chase:
                HandleChaseState();
                break;
            case NPCState.Shoot: // <-- Yeni durum
                HandleShootState();
                break;
        }

        UpdateAnimatorSpeed();
    }

    // Oyuncunun hareket edip etmedi�ini kontrol eden varsay�msal fonksiyon
    // NOT: Bu fonksiyonun do�ru �al��mas� i�in, oyuncu objenizde bir Rigidbody veya NavMeshAgent
    // bile�eninin olmas� ve hareket etti�inde h�z�n�n s�f�rdan b�y�k olmas� gerekir.
    bool IsPlayerIdle(Transform target)
    {
        // 1. Oyuncunun NavMeshAgent'� varsa, h�z�n� kontrol et
        NavMeshAgent playerAgent = target.GetComponent<NavMeshAgent>();
        if (playerAgent != null)
        {
            return playerAgent.velocity.magnitude < 0.1f;
        }

        // 2. Oyuncunun Rigidbody'si varsa, h�z�n� kontrol et
        Rigidbody playerRb = target.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            return playerRb.linearVelocity.magnitude < 0.1f;
        }

        // E�er oyuncu bir NavMeshAgent veya Rigidbody kullanm�yorsa,
        // bu mant�k do�ru �al��mayabilir. Bu durumda bu sat�r� kendi Player Controller kodunuza g�re d�zenlemelisiniz.
        return false; // Varsay�lan olarak hareket etti�ini varsayal�m.
    }


    // Oyuncu alg�lama ve durum de�i�tirme mant���n� bar�nd�r�r
    void CheckForPlayer()
    {
        // NPC'den sightRange kadar uzaktaki, PlayerMask'taki collider'lar� kontrol et
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, sightRange, playerMask);

        if (rangeChecks.Length > 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // 1. G�r�� a��s� ve engel kontrol� (Kesmeyi �nlemek i�in)
            if (Vector3.Angle(transform.forward, directionToTarget) < sightAngle / 2 &&
                !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, ~playerMask))
            {
                playerTarget = target; // Oyuncu g�r�ld�!

                // 2. YEN� MANTIK: At�� menzili ve Player'�n durumu
                if (distanceToTarget <= shootRange && IsPlayerIdle(target))
                {
                    ChangeState(NPCState.Shoot);
                }
                // 3. Mevcut durum Shoot de�ilse veya mesafesi Shoot menzili d���ndaysa Chase'e ge�
                else if (currentState != NPCState.Shoot)
                {
                    ChangeState(NPCState.Chase);
                }
            }
        }
        else // Menzilde oyuncu yoksa
        {
            playerTarget = null;
            // Kovalama veya At�� durumundayken oyuncu kaybolursa Devriyeye d�n
            if (currentState == NPCState.Chase || currentState == NPCState.Shoot)
            {
                ChangeState(NPCState.Patrol);
            }
        }
    }

    #region STATE HANDLERS

    void HandleInitialIdleState()
    {
        // ... (Ayn� kal�r) ...
        initialIdleTimer += Time.deltaTime;
        if (initialIdleTimer >= initialIdleDuration)
        {
            ChangeState(NPCState.Patrol);
        }
    }

    void HandlePatrolState()
    {
        // ... (Ayn� kal�r) ...
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0)
        {
            ChangeState(NPCState.RestIdle);
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (Time.time >= nextDestinationTime)
            {
                SetNewRandomDestination();
            }
        }
    }

    void HandleRestIdleState()
    {
        // ... (Ayn� kal�r) ...
        restIdleTimer += Time.deltaTime;
        if (restIdleTimer >= restIdleDuration)
        {
            ChangeState(NPCState.Patrol);
        }
    }

    void HandleChaseState()
    {
        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);

            // Kovalama durumunda NPC'nin s�rekli hedefe d�nmesini sa�la
            Vector3 lookDirection = playerTarget.position - transform.position;
            lookDirection.y = 0; // Y eksenini s�f�rla
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
        }
    }

    void HandleShootState()
    {
        // At�� durumunda NPC durur
        // NPC'nin s�rekli oyuncuya bakmas�n� sa�la
        if (playerTarget != null)
        {
            Vector3 lookDirection = playerTarget.position - transform.position;
            lookDirection.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
        }

        // At�� animasyonu oynat�ld�ktan sonra (Animasyon s�resi bitti�inde) 
        // otomatik olarak Idle/Chase durumuna d�nmek i�in bir Timer eklenebilir, 
        // ancak �imdilik Animator Controller'daki "Exit Time" bu g�revi g�recektir.
    }

    #endregion

    #region HELPER FUNCTIONS

    void ChangeState(NPCState newState)
    {
        // �nceki durumdan ��k��ta yap�lmas� gerekenler
        if (currentState == NPCState.Shoot)
        {
            // At�� bitince silah sesini durdurma vb.
        }

        currentState = newState;

        // Yeni duruma girerken yap�lmas� gerekenler
        if (newState == NPCState.Patrol)
        {
            agent.isStopped = false;
            agent.speed = patrolSpeed;
            patrolTimer = patrolDuration;
            SetNewRandomDestination();
        }
        else if (newState == NPCState.RestIdle || newState == NPCState.InitialIdle)
        {
            agent.isStopped = true; // Idle durumlar�nda hareket etmeyi durdur
            if (newState == NPCState.RestIdle) restIdleTimer = 0f;
        }
        else if (newState == NPCState.Chase)
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed; // H�z� kovalamaya y�kselt
        }
        else if (newState == NPCState.Shoot) // <-- Shooting durumu giri�i
        {
            agent.isStopped = true; // At�� yaparken dur
            animator.SetTrigger("ShootTrigger"); // At�� animasyonunu tetikle!
        }
    }

    void UpdateAnimatorSpeed()
    {
        if (currentState == NPCState.InitialIdle || currentState == NPCState.RestIdle || currentState == NPCState.Shoot)
        {
            animator.SetFloat("Speed", 0f);
        }
        else
        {
            float normalizedSpeed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("Speed", normalizedSpeed);
        }
    }

    // ... (SetNewRandomDestination ve GetRandomPoint ayn� kal�r) ...

    private void SetNewRandomDestination()
    {
        Vector3 randomPoint = GetRandomPoint(transform.position, walkRadius);
        agent.SetDestination(randomPoint);
        nextDestinationTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
    }

    private Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return center;
    }

    #endregion
}