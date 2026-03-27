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

        // If this is a thrown projectile, make sure collider is NOT a trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = false;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        direction.y = 0;
        startPosition = transform.position;

        Debug.Log("SetDirection called! Dir: " + direction + " | Speed: " + speed);

        //Kollar om rigibody finns och applicerar rörelse
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction.x * speed, 0f);
            Debug.Log("Velocity set to: " + rb.linearVelocity);
        }
        else
        {
            Debug.LogError("Rigidbody2D is NULL!");
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
            PokemonData pokemonData = collision.GetComponent<PokemonData>();
            HasBeenCaptured(collision.transform.position, pokemonData);
            Destroy(collision.gameObject);  // Förstör fienden EFTER att data sparats
            Destroy(gameObject);
        }
    }

    void HasBeenCaptured(Vector2 position, PokemonData pokemonData)
    {
        if (pokeCapture != null)
        {
            Vector2 pokeballSpawn = new(position.x, position.y + dropHeightOffset);
            GameObject pokeball = Instantiate(pokeCapture, pokeballSpawn, Quaternion.identity);

            // Get the pickup script (it should already be on the prefab)
            PokeBallPickup pickup = pokeball.GetComponent<PokeBallPickup>();

            if (pickup != null && pokemonData != null)
            {
                // Set the caught pokemon data directly
                pickup.SetCaughtPokemon(pokemonData);
                Debug.Log("✓ Fångade: " + pokemonData.GetPokemonName());
            }
        }
    }
}
