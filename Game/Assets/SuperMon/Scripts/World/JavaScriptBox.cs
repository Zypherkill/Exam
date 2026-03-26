using UnityEngine;

public class JavaScriptBox : MonoBehaviour
{
    public GameObject jsBoxPrefab;
    public Transform spawnPoint;
    public Animator animator;

    private bool isHit = false;

    private void Start()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHit)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if hit from below (jumping under the box)
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // When player hits from below, normal.y will be positive
                if (contact.normal.y > 0.5f)
                {
                    // Also check if player is moving upward (hitting with upward velocity)
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null && playerRb.linearVelocity.y > 0)
                    {
                        HandleHit();
                        break;
                    }
                }
            }
        }
    }

    void HandleHit()
    {
        isHit = true;

        if (animator != null)
        {
            animator.ResetTrigger("Hit"); // Clear any pending triggers
            animator.SetTrigger("Hit"); // Play hit animation immediately
        }

        // Spawn the prefab at the spawn point
        if (jsBoxPrefab != null && spawnPoint != null)
            Instantiate(jsBoxPrefab, spawnPoint.position, Quaternion.identity);
    }

    // Fallback: Also detect via trigger in case Rigidbody2D is Kinematic
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isHit)
            return;

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Trigger detected with player!");
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null && playerRb.linearVelocity.y > 0)
            {
                Debug.Log("Trigger: Player moving upward - triggering hit!");
                HandleHit();
            }
        }
    }
}