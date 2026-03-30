using UnityEngine;
using System.Collections;

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
	[SerializeField] private float preAttackDelay = 0.5f;
	[SerializeField] private float postAttackDelay = 0.5f;

	private Rigidbody2D rb;
	private Animator animator;
	private int direction = 1;
	private float attackTimer;
	private float turnTimer;
	private bool isDead;
	private bool isAttacking;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

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
		// Don't move during attack sequence
		if (isAttacking)
		{
			rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
			return;
		}

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
		if (attackTimer > 0f || turnTimer > 0f || isAttacking)
			return;

		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player == null)
			return;

		float distanceX = player.transform.position.x - transform.position.x;
		if (Mathf.Abs(distanceX) <= attackRange && Mathf.Sign(distanceX) == direction)
		{
			StartCoroutine(AttackSequence());
			attackTimer = attackCooldown;
		}
	}

	IEnumerator AttackSequence()
	{
		isAttacking = true;

		// Pre-attack delay (stop and prepare)
		yield return new WaitForSeconds(preAttackDelay);

		// Attack animation and projectile
		ShootWaterPush();

		// Post-attack delay (recovery)
		yield return new WaitForSeconds(postAttackDelay);

		isAttacking = false;
	}

	void ShootWaterPush()
	{
		// Start attack animation immediately
		if (animator != null)
			animator.SetBool("isAttacking", true);

		// Delay projectile to match animation
		StartCoroutine(DelayedShoot());
	}

	IEnumerator DelayedShoot()
	{
		yield return new WaitForSeconds(0.3f);

		if (projectilePrefab == null || firePoint == null)
			yield break;

		GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

		WaterPush projectile = proj.GetComponent<WaterPush>();
		if (projectile != null)
			projectile.SetDirection(new Vector2(direction, 0));

		// Stop attack animation
		StartCoroutine(StopAttackAnimation());
	}

	IEnumerator StopAttackAnimation()
	{
		yield return new WaitForSeconds(0.3f); // Adjust timing to match animation length
		if (animator != null)
			animator.SetBool("isAttacking", false);
	}

	public void TakeDamage()
	{
		if (isDead)
			return;

		isDead = true;
		StopAllCoroutines();

		if (animator != null)
		{
			animator.SetBool("isDead", true);
			StartCoroutine(DestroyAfterAnimation());
		}
		else
		{
			Destroy(gameObject);
		}
	}

	IEnumerator DestroyAfterAnimation()
	{
		// Wait for the frame to pass
		yield return new WaitForEndOfFrame();

		// Get animation clip length
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		float animationLength = stateInfo.length / stateInfo.speed;

		// Wait for animation to complete
		yield return new WaitForSeconds(animationLength);

		// Destroy the enemy
		Destroy(gameObject);
	}
}