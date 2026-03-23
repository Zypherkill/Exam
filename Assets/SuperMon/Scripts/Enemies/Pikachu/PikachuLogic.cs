using UnityEngine;
using System.Collections;

public class PikachuLogic : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float shootingRange = 4f;

    private Rigidbody2D rb;
    private Animator animator;
    private int direction = 1;
    private float attackTimer;
    private bool isDead;
    private bool isAttacking;

    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null)
        {
            Debug.LogError("PikachuLogic requires a Rigidbody2D.", this);
            enabled = false;
            return;
        }
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("PikachuLogic requires an Animator component.", this);
        }

        direction = Random.value > 0.5f ? 1 : -1;
        transform.localScale = new Vector3((float)direction, 1f, 1f);

        // Initialisera idle-animation
        isAttacking = false;
        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        // Uppdatera attacktimer
        attackTimer -= Time.fixedDeltaTime;
        UpdateFacingDirection();
        TryThunderbolt();
    }

    void UpdateFacingDirection()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        float distanceX = player.transform.position.x - transform.position.x;
        float distanceAbs = Mathf.Abs(distanceX);

        // Vänd mot spelaren om den är nära nog
        if (distanceAbs <= detectionRange)
        {
            int playerDirection = (int)Mathf.Sign(distanceX);
            if (playerDirection != 0)
                direction = playerDirection;
            transform.localScale = new Vector3((float)direction, 1f, 1f);
        }
    }

    void TryThunderbolt()
    {
        if (attackTimer > 0f)
            return;

        // Attackera inte om han redan attackerar
        if (isAttacking)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        float distanceX = player.transform.position.x - transform.position.x;
        float distanceAbs = Mathf.Abs(distanceX);

        // Skjut endast om spelaren är tillräckligt nära och framför Pikachu
        if (distanceAbs <= shootingRange && (int)Mathf.Sign(distanceX) == direction)
        {
            animator.SetBool("isAttacking", true);
            Thunderbolt();
            attackTimer = attackCooldown;
        }
    }

    void Thunderbolt()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        // Avbryt eventuella väntande coroutines
        StopCoroutine(nameof(ResetAttackAfterAnimation));

        // Ställ in attackanimation
        isAttacking = true;
        if (animator != null)
        {
            animator.SetBool("isAttacking", true);

            // Starta coroutine för att vänta på attackanimation
            StartCoroutine(ResetAttackAfterAnimation());
        }

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        ThunderboltProjectile projectile = proj.GetComponent<ThunderboltProjectile>();
        if (projectile != null)
            projectile.SetDirection(new Vector2((float)direction, 0));
    }

    IEnumerator ResetAttackAfterAnimation()
    {
        // Hämta animationsState
        if (animator == null)
            yield break;

        // Vänta på att attackanimation startar
        yield return new WaitForEndOfFrame();

        // Hämta animations-klippets längd och vänta tills den är klar
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length / stateInfo.speed;

        yield return new WaitForSeconds(animationLength);

        // Återställ attackState
        ResetAttackState();
    }

    void ResetAttackState()
    {
        // Återställ attackState
        isAttacking = false;
        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    public void TakeDamage()
    {
        // Spela attackanimation innan borttagning
        isDead = true;
        StopCoroutine(nameof(ResetAttackAfterAnimation));

        isAttacking = true;
        if (animator != null)
        {
            animator.SetBool("isAttacking", true);
            StartCoroutine(DestroyAfterAnimation());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterAnimation()
    {
        // Vänta på att attackanimation startar
        yield return new WaitForEndOfFrame();

        // Hämta animations-klippets längd
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length / stateInfo.speed;

        // Vänta tills animationen är klar
        yield return new WaitForSeconds(animationLength);

        // Radera objektet
        Destroy(gameObject);
    }
}
