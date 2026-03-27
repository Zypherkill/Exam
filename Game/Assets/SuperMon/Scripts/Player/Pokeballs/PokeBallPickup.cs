using UnityEngine;

public class PokeBallPickup : MonoBehaviour
{
    [SerializeField] private float pickupDelay = 0.5f;

    private float creationTime;
    private bool hasBeenPickedUp = false;
    private PokemonData caughtPokemon;

    void Start()
    {
        creationTime = Time.time;
    }

    public void SetCaughtPokemon(PokemonData pokemon)
    {
        caughtPokemon = pokemon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // En tid innan den kan plockas upp (för att undvika att plocka upp den för tidigt)
        if (Time.time - creationTime < pickupDelay)
            return;

        if (hasBeenPickedUp)
            return;

        // Kollar om det är spelaren som kolliderar
        if (collision.gameObject.CompareTag("Player"))
        {
            hasBeenPickedUp = true;
            Pickup();
        }
    }

    void Pickup()
    {
        if (Inventory.Instance != null)
        {
            if (caughtPokemon != null)
            {
                // Lägg till fångad Pokémon i inventoriet
                Inventory.Instance.CatchPokemon(caughtPokemon);
            }
            else
            {
                // Fallback - lägg bara till en normal pokeboll
                Inventory.Instance.AddPokeBall();
            }
        }

        Destroy(gameObject);
    }
}
