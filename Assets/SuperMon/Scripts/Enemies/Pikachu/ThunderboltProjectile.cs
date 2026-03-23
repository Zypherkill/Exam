using UnityEngine;

public class ThunderboltProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float beamLength = 4f;

    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        direction.y = 0;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        CheckBeamCollisions();
    }

    private void CheckBeamCollisions()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position,
            direction,
            beamLength
        );

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<PlayerHealth>()?.TakeDamage(transform.position.x);
                return;
            }
        }
    }
}