using UnityEngine;

public class WaterPush : MonoBehaviour
{
	[SerializeField] private float speed = 5f;
	[SerializeField] private float lifetime = 2f;
	[SerializeField] private float pushForce = 6f;

	private Vector2 direction;

	public void SetDirection(Vector2 dir)
	{
		direction = dir.normalized;
		direction.y = 0f;
	}

	void Start()
	{
		Destroy(gameObject, lifetime);
	}

	void Update()
	{
		transform.Translate(direction * speed * Time.deltaTime);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.CompareTag("Player"))
			return;

		PlayerHealth playerHealth = collision.GetComponentInParent<PlayerHealth>();
		playerHealth?.ApplyPush(transform.position.x, pushForce);

		Destroy(gameObject);
	}
}
