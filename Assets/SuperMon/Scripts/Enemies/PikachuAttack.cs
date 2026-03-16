using UnityEngine;
using UnityEngine.SceneManagement;

public class PikachuAttack : MonoBehaviour
{
    [SerializeField]
    private float attackRange = 1.5f;
    [SerializeField]
    private float attackDamage = 10f;
    [SerializeField]
    private float attackCooldown = 1f;
    [SerializeField]
    private LayerMask playerLayer;

    private GameObject player;
    private PlayerHealth playerHealth;
    private float attackTimer = 0f;

    private void Start()
    {
        // Lagrar referenser till spelaren och dess hälsa
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (player == null || playerHealth == null)
            return;

        // Förminskar attacktimern
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // Kollar om spelaren är inom attackområdet
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        
        if (distanceToPlayer <= attackRange && attackTimer <= 0)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }
    private void Attack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
