using UnityEngine;

public class JSCoin : MonoBehaviour
{
    public int pointsValue = 10;
    [SerializeField] private AudioClip coinSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PickupCoin();
        }
    }

    void PickupCoin()
    {
        // Play coin sound
        if (coinSound != null)
            AudioSource.PlayClipAtPoint(coinSound, transform.position);

        // Add points to score system
        if (ScoreSystem.instance != null)
            ScoreSystem.instance.AddPoints(pointsValue);

        // Destroy the coin
        Destroy(gameObject);
    }
}
