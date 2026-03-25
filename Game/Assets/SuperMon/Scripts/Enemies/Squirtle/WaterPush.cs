using UnityEngine;

public class WaterPush : MonoBehaviour
{
	[SerializeField] private float speed = 5f;
	[SerializeField] private float lifetime = 2f;
	[SerializeField] private float pushForce = 6f;
	[SerializeField] private float beamLength = 4f;

	private Vector2 direction;

	public void SetDirection(Vector2 dir)
	{
		direction = dir.normalized;
	}

	void Start()
	{
		Destroy(gameObject, lifetime);
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
				PlayerHealth playerHealth = hit.collider.GetComponentInParent<PlayerHealth>();
				playerHealth.ApplyPush(transform.position.x, pushForce);
				Destroy(gameObject);
				return;
			}
		}
	}
}
