using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    public float invincibilityDuration = 1.5f;
    public float knockbackForceX = 6f;
    public float knockbackForceY = 5f;

    public int currentLives;
    private bool isInvincible;
    private float invincibilityTimer;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        currentLives = maxLives;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isInvincible)
            return;

        invincibilityTimer -= Time.deltaTime;

        // flash the sprite while invincible
        if (Mathf.Sin(invincibilityTimer * 20f) > 0f)
            spriteRenderer.enabled = true;
        else
            spriteRenderer.enabled = false;

        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            spriteRenderer.enabled = true;
        }
    }

    // called by the enemy when it hits the player
    public void TakeDamage(float enemyX)
    {
        if (isInvincible)
            return;

        currentLives--;

        if (currentLives <= 0)
        {
            // no lives left, reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            // knock the player away from the enemy
            float knockDir = 1f;
            if (transform.position.x < enemyX)
                knockDir = -1f;

            rb.linearVelocity = new Vector2(knockDir * knockbackForceX, knockbackForceY);

            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    }
}

