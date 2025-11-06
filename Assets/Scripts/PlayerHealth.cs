using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    private float targetFill;

    [Header("UI References")]
    public Image healthBarFill;  // HealthBar_Fill objesini buraya sürük
    public float smoothSpeed = 5f;

    [Header("Shake Effect")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 5f;

    [Header("UI")]
    public GameObject GameOverText; // GameOverText objesini sürükle

    [Header("Damage Effect")]
    public Image hurtEffect;           // HurtEffect objesini sürükle
    public float hurtFadeSpeed = 2f;   // Solma hýzý
    public float hurtAlpha = 0.6f;     // Görünürlük seviyesi
    private Coroutine hurtCoroutine;


    private RectTransform barTransform;
    private Vector3 originalPos;
    private bool isShaking = false;
    public NavMeshAgent agent;
    private Animator animator;
    public AudioSource audioSource;
    public AudioClip hurtSound;
    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        targetFill = 1f;

        if (healthBarFill != null)
            barTransform = healthBarFill.GetComponent<RectTransform>();

        if (barTransform != null)
            originalPos = barTransform.localPosition;
        
    }

    void Update()
    {
        // Bar doluluðunu yumuþak animasyonla azalt
        healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFill, Time.deltaTime * smoothSpeed);

        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        targetFill = (float)currentHealth / maxHealth;

        if (hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        if (!isShaking)
            StartCoroutine(ShakeBar());

        Debug.Log("Player hasar aldý! Kalan can: " + currentHealth);

        if (hurtEffect != null)
        {
            if (hurtCoroutine != null)
                StopCoroutine(hurtCoroutine);
            hurtCoroutine = StartCoroutine(ShowHurtEffect());
        }

        if (currentHealth <= 0)
            Die();
        
       
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        targetFill = (float)currentHealth / maxHealth;
    }

    private IEnumerator ShakeBar()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            barTransform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        barTransform.localPosition = originalPos;
        isShaking = false;
    }

   
    void Die()
    {
        Debug.Log("Player died!");

        // Hareketi durdur
        if (agent != null)
            agent.isStopped = true;

        if (GameOverText != null)
            GameOverText.SetActive(true);

        // Animasyonu tetikle
        if (animator != null)
            animator.SetTrigger("Die");

        // PlayerControllerLogic scriptini devre dýþý býrak (hareket etmesin)
        PlayerControllerLogic controller = GetComponent<PlayerControllerLogic>();
        if (controller != null)
            controller.enabled = false;

        // Tekrar ölmemesi için component’leri kapatabiliriz
        GetComponent<Collider>().enabled = false;
        this.enabled = false; // PlayerHealth scriptini kapatýr

        // 3 saniye sonra objeyi kaldýr
        Destroy(gameObject, 3f);
    }

    private IEnumerator ShowHurtEffect()
    {
        Color color = hurtEffect.color;
        color.a = hurtAlpha;
        hurtEffect.color = color;

        // Yavaþça transparan hale getir
        while (color.a > 0f)
        {
            color.a -= Time.deltaTime * hurtFadeSpeed;
            hurtEffect.color = color;
            yield return null;
        }

        color.a = 0f;
        hurtEffect.color = color;
    }

}