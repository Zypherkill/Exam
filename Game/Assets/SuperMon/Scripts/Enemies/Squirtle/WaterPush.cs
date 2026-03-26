using UnityEngine;

public class WaterPush : MonoBehaviour
{
	[SerializeField] private float speed = 5f;
	[SerializeField] private float lifetime = 2f;
	[SerializeField] private float pushForce = 6f;
	[SerializeField] private float beamLength = 4f;

	private Vector2 direction;
	private SpriteRenderer spriteRenderer;
	private bool hasHit = false;

	public void SetDirection(Vector2 dir)
	{
		direction = dir.normalized;

		// Vända animationen baserat på riktning
		if (spriteRenderer == null)
			spriteRenderer = GetComponent<SpriteRenderer>();

		spriteRenderer.flipX = direction.x > 0;
	}

	void Start()
	{
		Destroy(gameObject, lifetime);
	}

	void Update()
	{
		transform.Translate(direction * speed * Time.deltaTime, Space.World);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		// Only hit once per projectile
		if (hasHit)
			return;

		if (collision.CompareTag("Player"))
		{
			hasHit = true;
			PlayerHealth playerHealth = collision.GetComponentInParent<PlayerHealth>();
			if (playerHealth != null)
			{
				float pushSourceX = collision.transform.position.x - direction.x * 10f;
				playerHealth.ApplyPush(pushSourceX, pushForce);
			}
			Destroy(gameObject);
		}
	}
}
