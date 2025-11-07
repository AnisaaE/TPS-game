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
    public Image healthBarFill;  // HealthBar_Fill Image dosyasý
    public float smoothSpeed = 5f;
    public Image DamageEffect;   // Canvas altýndaki DamageEffect objesindeki Image
    public Image DamageEffect1;

    [Header("Shake Effect")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 5f;

    [Header("UI")]
    public TextMeshProUGUI gameOverText;

    private RectTransform barTransform;
    private Vector3 originalPos;
    private bool isShaking = false;
    public NavMeshAgent agent;
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        gameOverText.gameObject.SetActive(false);
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

        if (!isShaking)
            StartCoroutine(ShakeBar());

        Debug.Log("Player hasar aldý! Kalan can: " + currentHealth);
        // Kan efekti kontrolü
        if (DamageEffect&& DamageEffect1 != null)
        {
            if (currentHealth > 10)
            {
                StartCoroutine(ShowBloodEffect(0.5f)); // 0.5 sn göster
            }
            else
            {
                DamageEffect.gameObject.SetActive(true); // 10 canýn altýndaysa hep açýk
                DamageEffect1.gameObject.SetActive(true);
            }
        }


        if (currentHealth <= 0 )
        {

            Die();
        }
        
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
        Debug.Log(" Player died!");

        animator.SetTrigger("Die");

        // Hareketi durdur
        GetComponent<PlayerControllerLogic>().enabled = false;
        // Game Over UI aktif et
        gameOverText.gameObject.SetActive(true);

        agent.isStopped = true;
        Destroy(gameObject,3f);
    }
    private IEnumerator ShowBloodEffect(float duration)
    {
        DamageEffect.gameObject.SetActive(true);
        DamageEffect1.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        if (currentHealth > 10)
        {
            DamageEffect.gameObject.SetActive(false);
            DamageEffect1.gameObject.SetActive(false);
        }
    }

}