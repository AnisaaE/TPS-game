using UnityEngine;
using UnityEngine.AI; // NavMeshAgent için gerekli kütüphane

public class Npc_AI : MonoBehaviour
{
    private NavMeshAgent agent;

    // NPC'nin kendi pozisyonundan ne kadar uzaða gidebileceðini belirler.
    public float walkRadius = 30f;

    // Yeni bir hedefe geçmeden önce ne kadar süre bekleyeceðini belirler.
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    private float nextDestinationTime;

    void Start()
    {
        // NavMeshAgent bileþenini al
        agent = GetComponent<NavMeshAgent>();

        // Agent bileþeni yoksa hata ver ve dur.
        if (agent == null)
        {
            Debug.LogError("NPC objesi üzerinde NavMeshAgent bileþeni bulunamadý!");
            enabled = false; // Script'i devre dýþý býrak
            return;
        }

        // Baþlangýçta bir hedef belirle
        SetNewRandomDestination();
    }

    void Update()
    {
        // Hedefe ulaþtýysa (kalan mesafe çok azsa) ve bekleme süresi dolduysa
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (Time.time >= nextDestinationTime)
            {
                SetNewRandomDestination();
            }
        }
    }

    // Rastgele bir NavMesh noktasý bulup hedefe ayarlayan ana fonksiyon
    private void SetNewRandomDestination()
    {
        // NPC'nin bulunduðu konum etrafýnda rastgele bir nokta bul
        Vector3 randomPoint = GetRandomPoint(transform.position, walkRadius);

        // Rastgele noktayý yeni hedef olarak ayarla
        agent.SetDestination(randomPoint);

        // Yeni hedef belirlendikten sonra rastgele bir bekleme süresi ayarla
        nextDestinationTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
    }

    // Belirli bir yarýçap içinde geçerli bir NavMesh noktasý bulan fonksiyon
    private Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        // Center etrafýnda rastgele bir yön al ve radius kadar uzaklaþtýr
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;

        NavMeshHit hit;
        // NavMesh'te, randomDirection pozisyonuna en yakýn geçerli bir pozisyon bul
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            // Eðer geçerli bir nokta bulunursa, o noktayý döndür
            return hit.position;
        }

        // Bir hata durumunda veya geçerli nokta bulunamazsa, NPC'nin kendi pozisyonunu döndür
        return center;
    }
}