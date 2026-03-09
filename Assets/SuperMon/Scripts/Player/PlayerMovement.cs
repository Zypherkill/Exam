using UnityEngine; // Import UnityEngine namespace, required for all Unity scripts

// All scripts that control a GameObject inherit from MonoBehaviour
public class PlayerMovement : MonoBehaviour
{
    // Public variables are visible in Unity Inspector
    public float moveSpeed = 5f;  // Player movement speed

    // Private variables are not visible in Inspector
    private Rigidbody2D rb;       // Reference to the Rigidbody2D component
    private Vector2 movement;     // Stores player movement input

    // Called once when the script is first run
    void Start()
    {
        // Get the Rigidbody2D component attached to the same GameObject
        rb = GetComponent<Rigidbody2D>();
    }

    // Called every frame, best for input handling
    void Update()
    {
        // Get horizontal and vertical input (WASD / Arrow keys)
        movement.x = Input.GetAxisRaw("Horizontal"); // -1 (left), 1 (right)
        movement.y = Input.GetAxisRaw("Vertical");   // -1 (down), 1 (up)
    }

    // Called every fixed frame-rate frame, best for physics movement
    void FixedUpdate()
    {
        // Move the player by setting Rigidbody2D velocity
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}