using UnityEngine;

public class Nikki : MonoBehaviour
{
    [SerializeField] private AudioClip musicToPlay;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Change the music
            if (musicToPlay != null && MusicManager.instance != null)
            {
                MusicManager.instance.PlayMusic(musicToPlay);
            }

            // Destroy efter en kort delay för att undvika lag
            Destroy(gameObject, 0.1f);
        }
    }
}
