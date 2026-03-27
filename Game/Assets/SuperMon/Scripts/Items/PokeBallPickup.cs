using UnityEngine;

public class PokeBallPickup : MonoBehaviour
{
    [SerializeField] private float pickupDelay = 0.5f;
    
    private float creationTime;
    private bool hasBeenPickedUp = false;

    void Start()
    {
        creationTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
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
            PickupPokeBall();
        }
    }

    void PickupPokeBall()
    {
        if (PokeBallInventory.Instance != null)
        {
            PokeBallInventory.Instance.AddPokeBall();
        }

        Destroy(gameObject);
    }
}
