using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthManager : MonoBehaviour
{
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public int maxHealth = 3;
    public int currentHealth;

    [SerializeField] private float flashDuration = 0.5f;
    [SerializeField] private float flashSpeed = 10f;

    private PlayerHealth playerHealth;
    private int lastDisplayedHealth;

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();

        // Get the actual current health from PlayerHealth, not the max
        if (playerHealth != null)
        {
            currentHealth = playerHealth.currentLives;
        }
        else
        {
            currentHealth = maxHealth;  // fallback if no PlayerHealth found
        }

        lastDisplayedHealth = currentHealth;
        UpdateHearts();
    }

    public void UpdateHealth(int newHealth)
    {
        int healthLost = currentHealth - newHealth;

        if (healthLost > 0)
        {
            // Flash the heart that was lost
            int lostHeartIndex = newHealth;  // The heart at this index just became empty
            if (lostHeartIndex >= 0 && lostHeartIndex < hearts.Length)
            {
                StartCoroutine(FlashHeart(hearts[lostHeartIndex]));
            }
        }

        currentHealth = newHealth;
        lastDisplayedHealth = currentHealth;
        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        lastDisplayedHealth = currentHealth;
        UpdateHearts();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        lastDisplayedHealth = currentHealth;
        UpdateHearts();
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }

    IEnumerator FlashHeart(Image heart)
    {
        float timer = 0f;
        Color originalColor = heart.color;

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Abs(Mathf.Sin(timer * flashSpeed)) * 255f;
            heart.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha / 255f);
            yield return null;
        }

        heart.color = originalColor;
    }
}
