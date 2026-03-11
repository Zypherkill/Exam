using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2 : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float jumpForce;

    private Rigidbody2D rb;

    private float moveInput;

    private bool isJumping;

    private bool isGrounded;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocityX = moveInput * moveSpeed;
        if (isJumping && isGrounded)
        {
            isJumping = false;
            rb.linearVelocityY = jumpForce;
        }

        if (moveInput != 0) {
            spriteRenderer.flipX = moveInput < 0;
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        CheckIfGrounded();
        isJumping = context.ReadValueAsButton();
    }

    public void CheckIfGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);
        if (hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
