using UnityEngine;
using UnityEngine.SceneManagement; // ако ще зареждаш нова сцена

public class EndGameTGR : MonoBehaviour
{
    public Animator playerAnimator;
    public Animator rumyAnimator;
    public GameObject endCanvas; // примерно UI за финала
    public float kissDelay = 1f;

    private bool hasEnded = false;
    public AudioSource danceMusic;

    private void OnTriggerEnter(Collider other)
    {
        if (hasEnded) return;

        if (other.CompareTag("Player"))
        {
            hasEnded = true;
            StartCoroutine(EndSequence());
        }
    }

    private System.Collections.IEnumerator EndSequence()
    {
        // Спиране на движението на играча
        var playerController = playerAnimator.GetComponent<PlayerControllerLogic>();

        playerAnimator.SetFloat("Speed", 0f);
      

        if (playerController != null)
            playerController.enabled = false;

        // Player и Rumy се обръщат един към друг
        Vector3 direction = rumyAnimator.transform.position - playerAnimator.transform.position;
        direction.y = 0;
        playerAnimator.transform.rotation = Quaternion.LookRotation(direction);
        rumyAnimator.transform.rotation = Quaternion.LookRotation(-direction);

        // Пускаме анимация за целувка
        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetTrigger("Dance");
        rumyAnimator.SetTrigger("Dance");
        // Dans mьziрini зal
        if (danceMusic != null && !danceMusic.isPlaying)
            danceMusic.Play();

        // Изчакваме анимацията
        yield return new WaitForSeconds(kissDelay);

        // Показваме край
        if (endCanvas != null)
            endCanvas.SetActive(true);

        // Или зареждаме нова сцена
        // SceneManager.LoadScene("GameOverScene");
    }
}

