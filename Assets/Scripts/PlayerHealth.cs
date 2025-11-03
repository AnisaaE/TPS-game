using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;

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

    private RectTransform barTransform;
    private Vector3 originalPos;
    private bool isShaking = false;
    public NavMeshAgent agent;
    void Start()
    {
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

        if (!isShaking)
            StartCoroutine(ShakeBar());

        Debug.Log("Player hasar aldý! Kalan can: " + currentHealth);

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
        Debug.Log(" Player died!");
        
        agent.isStopped = true;
        Destroy(gameObject,3f);
    }
    
}