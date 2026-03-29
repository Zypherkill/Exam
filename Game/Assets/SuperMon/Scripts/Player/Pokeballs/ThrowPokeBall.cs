using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowPokeBall : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float stillSpeedThreshold = 0.1f;
    [SerializeField] private float attackCooldown = 1f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private float cooldownTimer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Minska cooldown-timern
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void OnThrowInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ThrowProjectile();
        }
    }

    void ThrowProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        // Kontrollera cooldown
        if (cooldownTimer > 0)
            return;

        // Kontrollera att spelaren står still
        if (rb != null && Mathf.Abs(rb.linearVelocityX) > stillSpeedThreshold)
        {
            return;
        }

        // Kontrollera att det finns pokebollar i inventoryt
        if (Inventory.Instance == null || !Inventory.Instance.UsePokeBall())
        {
            Debug.Log("Inga pokebollar kvar!");
            return;
        }

        // Bestäm riktning baserat på transform scale
        float direction = transform.localScale.x < 0 ? -1f : 1f;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        PokeBallProjectile projectile = proj.GetComponent<PokeBallProjectile>();
        if (projectile != null)
        {
            projectile.SetDirection(new Vector2(direction, 0));
        }

        // Starta cooldown
        cooldownTimer = attackCooldown;
    }
}
