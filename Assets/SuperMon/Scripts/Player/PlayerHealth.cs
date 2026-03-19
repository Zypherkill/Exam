using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    public float invincibilityDuration = 1.5f;
    public float knockbackForceX = 6f;
    public float knockbackForceY = 5f;
    public float knockbackControlLockDuration = 0.2f;

    public int currentLives;
    private bool isInvincible;
    private float invincibilityTimer;
    private float knockbackControlLockTimer;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public bool IsKnockbackActive => knockbackControlLockTimer > 0f;

    void Start()
    {
        currentLives = maxLives;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (knockbackControlLockTimer > 0f)
            knockbackControlLockTimer -= Time.deltaTime;

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

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
            knockbackControlLockTimer = knockbackControlLockDuration;

            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    }

    public void ApplyPush(float sourceX, float horizontalForce)
    {
        if (rb == null)
            return;

        float pushDir = 1f;
        if (transform.position.x < sourceX)
            pushDir = -1f;

        rb.linearVelocity = new Vector2(pushDir * horizontalForce, rb.linearVelocity.y);
        knockbackControlLockTimer = knockbackControlLockDuration;
    }
}

