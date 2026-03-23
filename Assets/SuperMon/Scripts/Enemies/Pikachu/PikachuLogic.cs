using UnityEngine;

public class PikachuLogic : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackRange = 5f;

    private Rigidbody2D rb;

    private float attackTimer;
    private bool isDead;

    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("PikachuLogic requires a Rigidbody2D.", this);
            enabled = false;
            return;
        }
        animator = GetComponent<Animator>();

    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        attackTimer -= Time.fixedDeltaTime;
        TryThunderbolt();
    }

    void TryThunderbolt()
    {
        if (attackTimer > 0f)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        float distanceX = player.transform.position.x - transform.position.x;

        // Kolla om spelaren är inom range och framför fienden
        if (Mathf.Abs(distanceX) <= attackRange && Mathf.Sign(distanceX) == -1)
        {
            animator.SetBool("isAttacking", true);
            Thunderbolt();
            attackTimer = attackCooldown;
        }
    }

    void Thunderbolt()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        ThunderboltProjectile projectile = proj.GetComponent<ThunderboltProjectile>();
        if (projectile == null)
            return;

        projectile.SetDirection(new Vector2(-1, 0));
    }
}
