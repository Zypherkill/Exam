using UnityEngine;

public class SquirtleLogic : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float moveSpeed = 2f;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private float edgeLookAhead = 0.4f;
	[SerializeField] private float wallCheckDistance = 0.2f;

	[Header("Attack")]
	[SerializeField] private GameObject projectilePrefab;
	[SerializeField] private Transform firePoint;
	[SerializeField] private float attackCooldown = 1.5f;
	[SerializeField] private float attackRange = 5f;
	[SerializeField] private float turnCooldown = 0.2f;

	private Rigidbody2D rb;
	private int direction = 1;
	private float attackTimer;
	private float turnTimer;
	private bool isDead;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();

		if (Random.value > 0.5f)
			direction = 1;
		else
			direction = -1;
	}

	void FixedUpdate()
	{
		if (isDead)
			return;

		MoveAndTurn();
		attackTimer -= Time.fixedDeltaTime;
		turnTimer -= Time.fixedDeltaTime;
		TryWaterPush();
	}

	void MoveAndTurn()
	{
		rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

		transform.localScale = new Vector3(direction, 1f, 1f);

		Vector2 wallCheck = rb.position + new Vector2(direction * 0.4f, 0f);
		bool hitsWall = Physics2D.Raycast(wallCheck, Vector2.right * direction, wallCheckDistance, groundLayer);
		if (hitsWall)
		{
			direction *= -1;
			turnTimer = turnCooldown;
			return;
		}

		Vector2 edgeCheck = rb.position + new Vector2(direction * edgeLookAhead, 0.4f);
		bool hasGround = Physics2D.Raycast(edgeCheck, Vector2.down, 0.6f, groundLayer);
		if (!hasGround)
		{
			direction *= -1;
			turnTimer = turnCooldown;
		}
	}

	void TryWaterPush()
	{
		if (attackTimer > 0f || turnTimer > 0f)
			return;

		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player == null)
			return;

		float distanceX = player.transform.position.x - transform.position.x;
		if (Mathf.Abs(distanceX) <= attackRange && Mathf.Sign(distanceX) == direction)
		{
			ShootWaterPush();
			attackTimer = attackCooldown;
		}
	}

	void ShootWaterPush()
	{
		if (projectilePrefab == null || firePoint == null)
			return;

		GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

		WaterPush projectile = proj.GetComponent<WaterPush>();
		if (projectile == null)
			return;
		projectile.SetDirection(new Vector2(direction, 0));
	}
}
