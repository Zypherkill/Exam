using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2 : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float jumpTime = 0.1f;

    [SerializeField]
    private float stompForce;

    [SerializeField]
    private float runSpeedMultiplier = 1.5f;
    private Rigidbody2D rb;

    private float moveInput;

    private bool runInput;
    private bool jumpConsumed;
    private bool holdJump;
    private float jumpHoldTimer;
    private bool isGrounded = true;

    private Animator animator;
    private bool groundCheck;
    private PlayerHealth playerHealth;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private float defaultGravity = 3f;
    private float fallGravityMultiplier = 1f;

    [SerializeField]
    private float wallCheckDistance = 0.5f;
    private bool isAgainstWall = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void FixedUpdate()
    {
        GroundCheck();
        bool isKnockbackActive = playerHealth != null && playerHealth.IsKnockbackActive;
        if (!isKnockbackActive)
            rb.linearVelocityX = moveInput * moveSpeed;

        // Resettar hopptillstand nar man landar
        if (groundCheck && !isGrounded)
        {
            animator.SetBool("isJumping", false);
            holdJump = false;
            jumpHoldTimer = 0f;
        }
        isGrounded = groundCheck;

        // Hoppa endast om man ar pa marken
        if (jumpConsumed && groundCheck)
        {
            rb.linearVelocityY = jumpForce;
            holdJump = true;
            jumpHoldTimer = 0f;
            animator.SetBool("isJumping", true);
        }

        // Förläng hoppet under samma knapptryckning upp till max tid.
        if (holdJump && !groundCheck && jumpHoldTimer < jumpTime)
        {
            jumpHoldTimer += Time.fixedDeltaTime;
            rb.linearVelocityY = jumpForce;
        }

        // Ett nytt hopp kräver ett nytt knapptryck.
        jumpConsumed = false;

        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;

        if (!groundCheck || holdJump)
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", false);
        }
        else if (moveInput != 0)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", true);
            animator.SetBool("isIdle", false);
        }
        else if (jumpTime < jumpTime - 0.02f)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", true);
        }

        if (runInput && !isKnockbackActive)
        {
            rb.linearVelocityX = moveInput * moveSpeed * runSpeedMultiplier;
        }

    }

    //Funktion för att läsa in rörelse
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    //Funktion för att läsa in hopp
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        GroundCheck();
        bool currentHeld = context.ReadValueAsButton();
        if (currentHeld && !holdJump)
            jumpConsumed = true;

        if (!currentHeld)
            holdJump = currentHeld;
    }

    public void OnRunningInput(InputAction.CallbackContext context)
    {
        runInput = context.ReadValueAsButton();
    }

    //Funktion för att kolla om spelaren är på marken eller inte
    public void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        if (hit.collider != null)
        {
            groundCheck = true;
            rb.gravityScale = defaultGravity;
        }
        else
        {
            groundCheck = false;
            rb.gravityScale = defaultGravity * fallGravityMultiplier;
        }
    }

    //Funktion för att hoppa på en fiende och döda den
    public void StompBounce()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompForce);
        rb.AddForce(Vector2.up * 8, ForceMode2D.Impulse);
    }
}
