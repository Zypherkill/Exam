using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowPokeBall : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float stillSpeedThreshold = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
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

        // Kontrollera att spelaren står still
        if (rb != null && Mathf.Abs(rb.linearVelocityX) > stillSpeedThreshold)
        {
            return;
        }

        // Bestäm riktning baserat på SpriteRenderer.flipX
        float direction = spriteRenderer.flipX ? -1f : 1f;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        PokeBallProjectile projectile = proj.GetComponent<PokeBallProjectile>();
        if (projectile != null)
        {
            projectile.SetDirection(new Vector2(direction, 0));
        }
    }
}
