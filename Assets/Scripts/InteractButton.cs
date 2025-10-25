using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractButton : MonoBehaviour
    //Make interact button only appear when the player is close to the enemy
{
    public GameObject interactButton;

    public void Start()
    {
        if (interactButton != null)
        {
            interactButton.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactButton != null)
            {
                interactButton.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactButton != null)
            {
                interactButton.SetActive(false);
            }
        }
    }
}
