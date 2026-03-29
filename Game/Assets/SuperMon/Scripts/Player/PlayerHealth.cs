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
    [SerializeField] private AudioClip hurtSound;

    public int currentLives;
    private bool isInvincible;
    private float invincibilityTimer;
    private float knockbackControlLockTimer;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 spawnPosition;
    private AudioSource audioSource;

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
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spawnPosition = transform.position;  // Store the starting position

        // Set animator to alive state
        if (animator != null)
            animator.SetBool("isAlive", true);

        // Update UI on level load
        HealthManager healthManager = FindFirstObjectByType<HealthManager>();
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
        animator.SetTrigger("death");

        // Reset score on game over
        if (ScoreSystem.instance != null)
            ScoreSystem.instance.ResetScore();

        SceneManager.LoadScene("GameOver");
    }

    public void Respawn(Vector3 respawnPosition = default)
    {
        currentLives--;

        // Save health persistently
        PlayerPrefs.SetInt("PlayerHealth", currentLives);

        // Update the health UI
        HealthManager healthManager = FindFirstObjectByType<HealthManager>();
        if (healthManager != null)
            healthManager.UpdateHealth(currentLives);

        if (currentLives <= 0)
        {
            // no lives left, go to GameOver scene
            PlayerPrefs.DeleteKey("PlayerHealth");  // Reset health on game over

            // Reset score on game over
            if (ScoreSystem.instance != null)
                ScoreSystem.instance.ResetScore();

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
        TakeDamage(enemyX, "normal");
    }

    void PlayHurtSound()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null && hurtSound != null)
            audioSource.PlayOneShot(hurtSound);
    }

    // overload to specify damage type
    public void TakeDamage(float enemyX, string damageType)
    {
        // Allow final killing blow even if invincible, otherwise block damage during invincibility
        if (isInvincible && currentLives > 1)
            return;

        currentLives--;

        // Save health persistently
        PlayerPrefs.SetInt("PlayerHealth", currentLives);
        PlayerPrefs.Save();
        Debug.Log("Damage taken! Health saved: " + currentLives);

        // Trigger appropriate damage animation
        if (animator != null)
        {
            if (damageType == "thunder")
                animator.SetTrigger("takingThunderDamage");
            else
                animator.SetTrigger("takingNormalDamage");
        }

        // Update the health UI
        HealthManager healthManager = FindFirstObjectByType<HealthManager>();
        if (healthManager != null)
            healthManager.UpdateHealth(currentLives);

        if (currentLives <= 0)
        {
            PlayHurtSound();

            // Tell animator player is dead (so damage animations won't transition to idle)
            if (animator != null)
            {
                animator.SetBool("isAlive", false);
                animator.SetTrigger("isDying");
            }

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
            PlayHurtSound();
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
        PlayHurtSound();
    }

    // Play damage animation without dealing damage (used by non-damaging projectiles like WaterPush)
    public void PlayDamageAnimation(string damageType)
    {
        if (animator != null)
        {
            if (damageType == "thunder")
                animator.SetTrigger("takingThunderDamage");
            else
                animator.SetTrigger("takingNormalDamage");
        }
    }
}

