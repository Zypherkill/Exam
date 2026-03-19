using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2 : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float stompForce;

    [SerializeField]
    private float jumpTime = 0.01f;

    [SerializeField]
    private float runSpeedMultiplier = 1.5f;
    private Rigidbody2D rb;

    private float moveInput;

    private bool runInput;
    private bool jumpInput;
    private float jumpHoldTimer;
    private bool canJump = true;
    private bool isGrounded = true;
    private bool extendJump = false;

    private Animator animator;
    private bool groundCheck;
    private PlayerHealth playerHealth;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private float defaultGravity = 3f;
    private float fallGravityMultiplier = 1.2f;

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
        
        // Resettar canJump när man landar
        if (groundCheck && !isGrounded)
        {
            canJump = true;
            extendJump = false;
            animator.SetBool("isJumping", false);
        }
        isGrounded = groundCheck;
        // Check för att släppa hoppet om spelaren inte längre håller ner hoppknappen
        if (!jumpInput && extendJump)
        {
            extendJump = false;
        }
        
        // Hoppa endast om man är på marken och inga dubbelhopp är tillåtna
        if (jumpInput && groundCheck && canJump)
        {
            jumpHoldTimer = 0;
            rb.linearVelocityY = jumpForce;
            canJump = false;
            extendJump = true;
            animator.SetBool("isJumping", true);
        }

        // Förläng hoppet så länge spelaren håller ner hoppknappen och inte har nått max jump time
        if (extendJump && !groundCheck && jumpHoldTimer < jumpTime)
        {
            jumpHoldTimer += Time.deltaTime;
            float force = (jumpHoldTimer >= jumpTime) ? jumpForce * 2f : jumpForce;
            rb.linearVelocityY = force;
        }

        if (moveInput != 0) {
            spriteRenderer.flipX = moveInput < 0;
            animator.SetBool("isRunning", true);
        } else {
            animator.SetBool("isRunning", false);
        }

        if(moveSpeed !=0 && jumpInput)
        {
            animator.SetBool("isJumping", true);
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
        jumpInput = context.ReadValueAsButton();
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
