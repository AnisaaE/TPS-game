using UnityEngine;
using System.Collections;

public class EndGameTrigger : MonoBehaviour
{
    [Header("Animators")]
    public Animator playerAnimator;
    public Animator rumyAnimator;

    [Header("UI")]
    public GameObject endKissCanvas;  // Canvas с надпис "THE END 💋"

    [Header("Settings")]
    public float kissDelay = 1f; // колко секунди трае анимацията на целувката

    private bool hasEnded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasEnded) return;

        // Проверка дали влезлият обект е Player
        if (other.CompareTag("Player"))
        {
            hasEnded = true;
            StartCoroutine(EndSequence());
        }
    }

    private IEnumerator EndSequence()
    {
        // Спиране на движението на Player (ако скриптът му е PlayerControllerLogic)
        var playerController = playerAnimator.GetComponent<PlayerControllerLogic>();
        if (playerController != null)
            playerController.enabled = false;

        // Player и Rumy се обръщат един към друг
        Vector3 direction = rumyAnimator.transform.position - playerAnimator.transform.position;
        direction.y = 0;
        playerAnimator.transform.rotation = Quaternion.LookRotation(direction);
        rumyAnimator.transform.rotation = Quaternion.LookRotation(-direction);

        // Изчакване преди анимация
        yield return new WaitForSeconds(0.5f);

        // Стартиране на анимация за целувка
        playerAnimator.SetTrigger("Kiss");
        rumyAnimator.SetTrigger("Kiss");

        // Изчакване докато анимацията на целувката завърши
        yield return new WaitForSeconds(kissDelay);

        // Показване на финалния Canvas
        if (endKissCanvas != null)
            endKissCanvas.SetActive(true);
    }
}
