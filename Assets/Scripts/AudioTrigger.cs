using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public GameObject HitAudio;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag("Player"))
        {
            GetComponent<AudioSource>().Play();
        }
        if (gameObject.CompareTag("Enemy"))
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
