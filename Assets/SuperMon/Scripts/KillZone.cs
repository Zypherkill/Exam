using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Respawn();
        }
    }
}

