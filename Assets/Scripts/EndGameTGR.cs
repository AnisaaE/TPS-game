using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTGR : MonoBehaviour
{
    public Animator playerAnimator;
    public Animator rumyAnimator;
    public GameObject endCanvas; 
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
        
        var playerController = playerAnimator.GetComponent<PlayerControllerLogic>();
        
        
        if (playerController != null)
            playerController.StopAllAudio();

        playerAnimator.SetFloat("Speed", 0f);
      

        if (playerController != null)
            playerController.enabled = false;

        Vector3 direction = rumyAnimator.transform.position - playerAnimator.transform.position;
        direction.y = 0;
        playerAnimator.transform.rotation = Quaternion.LookRotation(direction);
        rumyAnimator.transform.rotation = Quaternion.LookRotation(-direction);

        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetTrigger("Dance");
        rumyAnimator.SetTrigger("Dance");
        
        if (danceMusic != null && !danceMusic.isPlaying)
            danceMusic.Play();

        yield return new WaitForSeconds(kissDelay);

        if (endCanvas != null)
        {
            endCanvas.SetActive(true);
        }
    }
}

