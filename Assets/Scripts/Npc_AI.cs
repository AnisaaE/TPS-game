using UnityEngine;
using UnityEngine.AI; // NavMeshAgent i�in gerekli k�t�phane

public class Npc_AI : MonoBehaviour
{
    private NavMeshAgent agent;

    // NPC'nin kendi pozisyonundan ne kadar uza�a gidebilece�ini belirler.
    public float walkRadius = 30f;

    // Yeni bir hedefe ge�meden �nce ne kadar s�re bekleyece�ini belirler.
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    private float nextDestinationTime;

    void Start()
    {
        // NavMeshAgent bile�enini al
        agent = GetComponent<NavMeshAgent>();

        // Agent bile�eni yoksa hata ver ve dur.
        if (agent == null)
        {
            Debug.LogError("NPC objesi �zerinde NavMeshAgent bile�eni bulunamad�!");
            enabled = false; // Script'i devre d��� b�rak
            return;
        }

        // Ba�lang��ta bir hedef belirle
        SetNewRandomDestination();
    }

    void Update()
    {
        // Hedefe ula�t�ysa (kalan mesafe �ok azsa) ve bekleme s�resi dolduysa
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (Time.time >= nextDestinationTime)
            {
                SetNewRandomDestination();
            }
        }
    }

    // Rastgele bir NavMesh noktas� bulup hedefe ayarlayan ana fonksiyon
    private void SetNewRandomDestination()
    {
        // NPC'nin bulundu�u konum etraf�nda rastgele bir nokta bul
        Vector3 randomPoint = GetRandomPoint(transform.position, walkRadius);

        // Rastgele noktay� yeni hedef olarak ayarla
        agent.SetDestination(randomPoint);

        // Yeni hedef belirlendikten sonra rastgele bir bekleme s�resi ayarla
        nextDestinationTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
    }

    // Belirli bir yar��ap i�inde ge�erli bir NavMesh noktas� bulan fonksiyon
    private Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        // Center etraf�nda rastgele bir y�n al ve radius kadar uzakla�t�r
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;

        NavMeshHit hit;
        // NavMesh'te, randomDirection pozisyonuna en yak�n ge�erli bir pozisyon bul
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            // E�er ge�erli bir nokta bulunursa, o noktay� d�nd�r
            return hit.position;
        }

        // Bir hata durumunda veya ge�erli nokta bulunamazsa, NPC'nin kendi pozisyonunu d�nd�r
        return center;
    }
}