using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
    private Vector3 spawnPosition;

    public bool IsKnockbackActive => knockbackControlLockTimer > 0f;

    void Start()
    {
        // Load health from persistent storage, or use max if first time
        if (PlayerPrefs.HasKey("PlayerHealth"))
        {
            currentLives = PlayerPrefs.GetInt("PlayerHealth");
            Debug.Log("Loaded health from PlayerPrefs: " + currentLives);
        }
        else
        {
            currentLives = maxLives;
            Debug.Log("No saved health, starting with max: " + maxLives);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;  // Store the starting position

        // Update UI on level load
        HealthManager healthManager = FindObjectOfType<HealthManager>();
        if (healthManager != null)
            healthManager.UpdateHealth(currentLives);
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
        SceneManager.LoadScene("GameOver");
    }

    public void Respawn(Vector3 respawnPosition = default)
    {
        currentLives--;

        // Save health persistently
        PlayerPrefs.SetInt("PlayerHealth", currentLives);

        // Update the health UI
        HealthManager healthManager = FindObjectOfType<HealthManager>();
        if (healthManager != null)
            healthManager.UpdateHealth(currentLives);

        if (currentLives <= 0)
        {
            // no lives left, go to GameOver scene
            PlayerPrefs.DeleteKey("PlayerHealth");  // Reset health on game over
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            // Move player back to spawn position
            transform.position = spawnPosition;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Apply invincibility on respawn
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
            knockbackControlLockTimer = 0f;  // Reset knockback lock
        }
    }

    // called by the enemy when it hits the player
    public void TakeDamage(float enemyX)
    {
        // Allow final killing blow even if invincible, otherwise block damage during invincibility
        if (isInvincible && currentLives > 1)
            return;

        currentLives--;

        // Save health persistently
        PlayerPrefs.SetInt("PlayerHealth", currentLives);
        PlayerPrefs.Save();
        Debug.Log("Damage taken! Health saved: " + currentLives);

        // Update the health UI
        HealthManager healthManager = FindObjectOfType<HealthManager>();
        if (healthManager != null)
            healthManager.UpdateHealth(currentLives);

        if (currentLives <= 0)
        {
            // no lives left, wait before going to GameOver scene
            PlayerPrefs.DeleteKey("PlayerHealth");
            Debug.Log("Game Over! Health reset.");
            StartCoroutine(WaitBeforeGameOver());
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

    IEnumerator WaitBeforeGameOver()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("GameOver");
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

