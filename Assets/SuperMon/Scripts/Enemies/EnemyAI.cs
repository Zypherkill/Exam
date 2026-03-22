using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public float edgeLookAhead = 0.4f;
    public float wallCheckDistance = 0.2f;

    private Rigidbody2D rb;
    private Collider2D col;

    private bool isDead;
    private float damageCooldown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        if (rb == null || col == null)
        {
            Debug.LogError("EnemyAI requires Rigidbody2D and Collider2D on the same GameObject.", this);
            enabled = false;
            return;
        }

        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (isDead)
            return;


        CheckForPlayer();

        if (damageCooldown > 0f)
            damageCooldown -= Time.fixedDeltaTime;

        // stop physics from launching the enemy upward when hit
        if (rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

    }

    public void CheckForPlayer()
    {
        if (col == null)
            return;

        // Expand the check area slightly upward so top contacts are included.
        Vector2 checkSize = new Vector2(col.bounds.size.x, col.bounds.size.y + 0.2f);
        Vector2 checkCenter = new Vector2(col.bounds.center.x, col.bounds.center.y + 0.1f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(checkCenter, checkSize, 0f);
        PlayerHealth playerHealth = null;

        for (int i = 0; i < hits.Length; i++)
        {
            playerHealth = hits[i].GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                break;
            }
        }

        if (playerHealth == null)
            return;

        GameObject player = playerHealth.gameObject;
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

        bool playerAbove = player.transform.position.y > transform.position.y + 0.2f;
        bool playerFalling = playerRb != null && playerRb.linearVelocity.y < 0f;

        if (playerAbove && playerFalling)
        {
            // player jumped on top of the enemy, kill it
            KillEnemy(player);
        }
        else if (damageCooldown <= 0f)
        {
            // enemy touches player from the side, damage the player
            playerHealth.TakeDamage(transform.position.x);
            damageCooldown = 0.5f;
        }
    }

    void KillEnemy(GameObject player)
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        col.enabled = false;

        // bounce the player up
        PlayerMovement2 movement = player.GetComponentInParent<PlayerMovement2>();
        movement.StompBounce();

        // Play attack animation if stomped on, its similar to a death animation.
        PikachuLogic pikachu = GetComponent<PikachuLogic>();
        if (pikachu != null)
        {
            pikachu.TakeDamage();
        }
        else
        {
            Destroy(gameObject, 0.3f);
        }
    }

}