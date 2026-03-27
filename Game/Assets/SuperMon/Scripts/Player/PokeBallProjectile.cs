using UnityEngine;

public class PokeBallProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float travelDistance = 5f;
    [SerializeField] private GameObject pokeCapture;
    [SerializeField] private float dropHeightOffset = 1.5f;

    private Vector2 direction;
    private Rigidbody2D rb;
    private Vector3 startPosition;
    private bool isCaptured = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        direction.y = 0;
        startPosition = transform.position;
        
        //Kollar om rigibody finns och applicerar rörelse
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
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
        if (isCaptured)
            return;

        // Träffa fiender - förstör både fienden och projektilen
        if (collision.CompareTag("Enemy"))
        {
            isCaptured = true;
            HasBeenCaptured(collision.transform.position);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    void HasBeenCaptured(Vector2 position)
    {
        if (pokeCapture != null)
        {
            Vector2 pokeballSpawn = new (position.x, position.y + dropHeightOffset);
            GameObject pokeball = Instantiate(pokeCapture, pokeballSpawn, Quaternion.identity);
            
            // Lägg till pickup-scriptet om det inte redan finns
            if (pokeball.GetComponent<PokeBallPickup>() == null)
            {
                pokeball.AddComponent<PokeBallPickup>();
            }
        }
    }
}
