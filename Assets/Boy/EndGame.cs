using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTrigger : MonoBehaviour
{
    public Animator princeAnimator; // сложи тук аниматора на принца
    public Animator playerAnimator; // ако искаш да пуснеш и анимация на героинята
    public string endSceneName = "EndScene"; // следваща сцена

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Героинята намери принца 💖");

            // Спиране на движението на героинята (ако имаш контролер)
            var controller = other.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            // Анимация на принца (примерно да се събуди)
            if (princeAnimator != null)
                princeAnimator.SetTrigger("WakeUp");

            // Анимация на героинята (примерно целувка)
            if (playerAnimator != null)
                playerAnimator.SetTrigger("Kiss");

            // След малко — край на играта
            Invoke("EndGame", 3f);
        }
    }

    void EndGame()
    {
        SceneManager.LoadScene(endSceneName);
    }
}

