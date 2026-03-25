using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float jumpForce = 16f;
    public float stompBounce = 10f;
    public float fallGravityMultiplier = 3f;
    public float jumpBufferTime = 0.15f;

    // drag a small empty child object placed just under the player's feet into this
    public Transform groundCheck;
    public float groundCheckDistance = 0.05f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private float defaultGravity;
    private bool isGrounded;
    private bool jumpConsumed;
    private float jumpBufferTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        defaultGravity = rb.gravityScale;
    }

    void Update()
    {
        // check if we're on the ground
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        // allow jumping again once we land
        if (isGrounded && rb.linearVelocity.y <= 0f)
        {
            jumpConsumed = false;
            animator.SetBool("isJumping", false);
        }

        // remember jump input for a short window so it doesn't get missed
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumpBufferTimer = jumpBufferTime;
            animator.SetBool("isJumping", true);
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
            animator.SetBool("isRunning", false);

        }

        // jump if grounded and haven't already jumped
        if (jumpBufferTimer > 0f && isGrounded && !jumpConsumed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferTimer = 0f;
            jumpConsumed = true;
        }

        // if player lets go early, cut the jump short
        if ((Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.UpArrow)) && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        // flip sprite left/right - setup for animations later, but it works for now
        float h = Input.GetAxisRaw("Horizontal");
        if (h > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (h < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (animator != null)
        {
            animator.SetBool("isRunning", h != 0f && isGrounded);
        }
    }

    void FixedUpdate()
    {
        // move left and right
        float h = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);

        // make falling feel heavier
        if (rb.linearVelocity.y < 0f && !isGrounded)
        {
            rb.gravityScale = defaultGravity * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }
    }

    // called when the player stomps an enemy
    public void StompBounce()
    {
        jumpConsumed = true;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounce);
    }
}
