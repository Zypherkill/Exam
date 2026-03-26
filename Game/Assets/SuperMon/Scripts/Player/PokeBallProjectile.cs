using UnityEngine;

public class PokeBallProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float travelDistance = 5f;

    private Vector2 direction;
    private Rigidbody2D rb;
    private Vector3 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    public void SetDirection(Vector2 dir, Vector2 playerVelocity = default)
    {
        direction = dir.normalized;
        direction.y = 0;
        startPosition = transform.position;
        
        //Kollar om rigibody finns och applicerar rörelse
        if (rb != null)
        {
            // Lägg till spelarens X-hastighet för naturligare rörelse
            float finalSpeed = speed + Mathf.Abs(playerVelocity.x);
            rb.linearVelocity = new Vector2(direction.x * finalSpeed, rb.linearVelocity.y);
        }
    }

    void Update()
    {
        float distanceTraveled = Vector3.Distance(transform.position, startPosition);
        if (distanceTraveled >= travelDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignorera spelaren
        if (collision.CompareTag("Player"))
            return;

        // Träffa fiender
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignorera spelaren
        if (collision.gameObject.CompareTag("Player"))
            return;

        // Förstörs vid kollision med vägg/mark osv
        Destroy(gameObject);
    }
}
