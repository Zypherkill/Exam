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
    private int direction = 1;
    private bool isDead;
    private float damageCooldown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.freezeRotation = true;

        // randomly pick a starting direction
        if (Random.value > 0.5f)
            direction = 1;
        else
            direction = -1;
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        MoveAndTurn();
        CheckForPlayer();

        if (damageCooldown > 0f)
            damageCooldown -= Time.fixedDeltaTime;

        // stop physics from launching the enemy upward when hit
        if (rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    }

    void MoveAndTurn()
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // flip sprite to face movement direction
        transform.localScale = new Vector3(direction, 1f, 1f);

        // turn around if there's a wall ahead
        Vector2 wallCheck = rb.position + new Vector2(direction * 0.4f, 0f);
        bool hitsWall = Physics2D.Raycast(wallCheck, Vector2.right * direction, wallCheckDistance, groundLayer);
        if (hitsWall)
        {
            direction = direction * -1;
            return;
        }

        // turn around if there's no ground ahead (so we don't walk off edges)
        Vector2 edgeCheck = rb.position + new Vector2(direction * edgeLookAhead, -0.2f);
        bool hasGround = Physics2D.Raycast(edgeCheck, Vector2.down, 0.6f, groundLayer);
        if (!hasGround)
        {
            direction = direction * -1;
        }
    }

    public void CheckForPlayer()
    {
        // see if the player is overlapping with this enemy
        Collider2D playerCol = Physics2D.OverlapBox(col.bounds.center, col.bounds.size, 0f, playerLayer);
        if (playerCol == null)
            return;

        GameObject player = playerCol.gameObject;
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
            player.GetComponent<PlayerHealth>().TakeDamage(transform.position.x);
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
        player.GetComponent<PlayerMovement2>().StompBounce();

        Destroy(gameObject, 0.3f);
    }
}

