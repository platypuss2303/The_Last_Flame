using UnityEngine;
using System.Collections;

public class summoned : MonoBehaviour
{
    public int maxHealth = 5;
    public Animator animator;
    public Transform player;
    public float attackRange = 6f;
    private bool playerInRange = false;
    public float walkSpeed = 1.8f;
    public float chaseSpeed = 2.8f;
    public float retrieveDistance = 2f;

    public Transform detectPoint;
    public float distance = 1f;
    public LayerMask detectLayer;
    private bool facingLeft = true;

    public Transform attackPoint;
    public float attackRadius = 1.5f;
    public LayerMask attackLayer;

    private bool isAttacking = false;
    public float attackDelay = 1.2f;
    private bool isPlayerDead = false;

    private bool isInDamageCooldown = false;
    private float damageCooldownDuration = 0.5f;

    // Sử dụng tên biến khác để tránh xung đột
    public GameObject summonedPrefab;
    private bool hasSummoned = false;

    // Biến tuần tra
    public float patrolDistance = 4f;
    private Vector2 startPosition;
    private float distanceTraveled;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator không tìm thấy trên " + gameObject.name);

        // Tìm GameObject với tag "Player_Level3" thay vì "Player"
        player = GameObject.FindGameObjectWithTag("Player_Level3")?.transform;
        if (player == null) Debug.LogError("Player_Level3 không tìm thấy! Vui lòng đảm bảo Player_Level3 có tag 'Player_Level3'.");

        startPosition = transform.position;
    }

    void Update()
    {
        if (maxHealth <= 0)
        {
            Die();
            return;
        }

        if (isPlayerDead || player == null || !player.gameObject.activeSelf)
        {
            animator.SetBool("PlayerDead", true);
            return;
        }

        animator.SetBool("PlayerDead", false);
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerInRange = distanceToPlayer <= attackRange;

        if (!playerInRange)
        {
            animator.SetBool("Attack", false);
            Patrol();
        }
        else
        {
            FacePlayer();
            if (distanceToPlayer > retrieveDistance && !isInDamageCooldown)
            {
                animator.SetBool("Attack", false);
                ChasePlayer(distanceToPlayer);
            }
            else if (!isAttacking && !isInDamageCooldown)
            {
                StartCoroutine(AttackRoutine());
            }
        }

        // Kích hoạt summon khi tấn công gần
        if (playerInRange && distanceToPlayer <= attackRadius && !hasSummoned)
        {
            SummonSkeleton();
        }
    }

    void Patrol()
    {
        if (detectPoint == null)
        {
            Debug.LogError("DetectPoint chưa được gán trên " + gameObject.name);
            return;
        }

        transform.Translate(Vector2.left * walkSpeed * Time.deltaTime);
        distanceTraveled = Mathf.Abs(transform.position.x - startPosition.x);

        RaycastHit2D hit = Physics2D.Raycast(detectPoint.position, Vector2.down, distance, detectLayer);
        if (distanceTraveled >= patrolDistance || !hit)
        {
            if (facingLeft)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingLeft = true;
            }
            startPosition = transform.position;
        }
    }

    void FacePlayer()
    {
        if (player == null) return;

        if (transform.position.x < player.position.x && facingLeft)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingLeft = false;
        }
        else if (transform.position.x > player.position.x && !facingLeft)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingLeft = true;
        }
    }

    void ChasePlayer(float distanceToPlayer)
    {
        if (player == null) return;
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("Attack", true);

        yield return new WaitForSeconds(attackDelay);

        Attack();
        animator.SetBool("Attack", false);
        isAttacking = false;
    }

    public void Attack()
    {
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint chưa được gán trên " + gameObject.name);
            return;
        }

        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collInfo && collInfo.GetComponent<Player_Level3>() != null)
        {
            collInfo.GetComponent<Player_Level3>().Player_Level3TakeDamage(1);
        }
    }

    void SummonSkeleton()
    {
        if (!hasSummoned)
        {
            for (int i = 0; i < 1; i++) // Summon 1 skeleton
            {
                Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), -1f, 0);
                Vector3 summonPosition = transform.position + offset;
                Instantiate(summonedPrefab, summonPosition, Quaternion.identity);
            }
            hasSummoned = true;
        }
    }

    public void EnemyTakeDamage(int damage)
    {
        if (maxHealth <= 0 || isInDamageCooldown) return;
        maxHealth -= damage;
        Debug.Log(gameObject.name + " nhận sát thương, HP còn: " + maxHealth);

        animator.SetBool("Damage", true);
        isInDamageCooldown = true;
        Invoke("EndDamageCooldown", damageCooldownDuration);
    }

    private void EndDamageCooldown()
    {
        isInDamageCooldown = false;
        animator.SetBool("Damage", false);
    }

    public void OnPlayerDead()
    {
        isPlayerDead = true;
        player = null;
        animator.SetBool("PlayerDead", true);
        StartCoroutine(TransitionToIdleAfterDelay(1.0f));
        Debug.Log(gameObject.name + " nhận biết Player đã chết, chuyển sang trạng thái Idle.");
    }

    private IEnumerator TransitionToIdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("PlayerDead", false);
        animator.SetBool("Attack", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            Die();
            Debug.Log(gameObject.name + " fell into DeathZone!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (detectPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(detectPoint.position, Vector2.down * distance);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " Died");
        Destroy(gameObject);
    }
}