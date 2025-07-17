using UnityEngine;
using System.Collections;

public class MinotaurGirl : MonoBehaviour
{
    public int maxHealth = 3;
    public Animator animator;
    public Transform player_Level3;
    public float attackRange = 10f;
    private bool player_Level3InRange = false;
    public float runSpeed = 2.5f;
    public float chaseSpeed = 3.5f;
    public float retrieveDistance = 2.5f;

    public Transform detectPoint;
    public float distance = 1f; // Giá trị mặc định để tránh lỗi
    public LayerMask detectLayer;
    private bool facingLeft = true;

    public Transform attackPoint;
    public float attackRadius = 2f;
    public LayerMask attackLayer;

    private bool isAttacking = false;
    public float attackDelay = 1.5f;
    private bool isPlayer_Level3Dead = false;

    private bool isInDamageCooldown = false;
    private float damageCooldownDuration = 0.5f;

    public float patrolDistance = 5f;
    private Vector2 startPosition;
    private float distanceTraveled;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator không tìm thấy trên " + gameObject.name);
        }

        player_Level3 = GameObject.FindGameObjectWithTag("Player_Level3")?.transform;
        if (player_Level3 == null)
        {
            Debug.LogError("Player_Level3 không tìm thấy! Vui lòng đảm bảo Player_Level3 có tag 'Player_Level3'.");
        }

        startPosition = transform.position;
    }

    void Update()
    {
        if (maxHealth <= 0)
        {
            Die();
            return;
        }

        if (isPlayer_Level3Dead || player_Level3 == null || (player_Level3 != null && !player_Level3.gameObject.activeSelf))
        {
            animator.SetBool("PlayerDead", true);
            return;
        }

        animator.SetBool("PlayerDead", false);
        if (player_Level3 != null)
        {
            float distanceToPlayer_Level3 = Vector2.Distance(transform.position, player_Level3.position);
            player_Level3InRange = distanceToPlayer_Level3 <= attackRange;
            Debug.Log($"Distance to Player: {distanceToPlayer_Level3}, In Range: {player_Level3InRange}, Cooldown: {isInDamageCooldown}, Attacking: {isAttacking}");

            if (!player_Level3InRange)
            {
                animator.SetBool("Attack", false);
                Patrol();
            }
            else
            {
                FacePlayer_Level3();
                // Chỉ chase nếu cách xa hơn retrieveDistance, nếu không thì tấn công
                if (distanceToPlayer_Level3 > retrieveDistance && !isInDamageCooldown)
                {
                    animator.SetBool("Attack", false);
                    ChasePlayer_Level3(distanceToPlayer_Level3);
                }
                else if (distanceToPlayer_Level3 <= retrieveDistance && !isAttacking && !isInDamageCooldown)
                {
                    animator.SetBool("Attack", true);
                    StartCoroutine(AttackRoutine());
                    Debug.Log("Starting Attack");
                }
            }
        }
    }

    void Patrol()
    {
        if (detectPoint == null)
        {
            Debug.LogError("DetectPoint chưa được gán trên " + gameObject.name);
            return;
        }

        transform.Translate(Vector2.left * runSpeed * Time.deltaTime);
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

    void FacePlayer_Level3()
    {
        if (player_Level3 == null) return;

        if (transform.position.x < player_Level3.position.x && facingLeft)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingLeft = false;
        }
        else if (transform.position.x > player_Level3.position.x && !facingLeft)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingLeft = true;
        }
    }

    void ChasePlayer_Level3(float distanceToPlayer_Level3)
    {
        if (player_Level3 == null) return;
        transform.position = Vector2.MoveTowards(transform.position, player_Level3.position, chaseSpeed * Time.deltaTime);
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("Attack", true);

        yield return new WaitForSeconds(attackDelay);

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

    public void OnPlayer_Level3Dead()
    {
        isPlayer_Level3Dead = true;
        player_Level3 = null;
        animator.SetBool("PlayerDead", true);
        StartCoroutine(TransitionToIdleAfterDelay(1.0f));
        Debug.Log(gameObject.name + " nhận biết Player_Level3 đã chết, chuyển sang trạng thái Idle.");
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
        GameManager_Level3 gameManager = FindFirstObjectByType<GameManager_Level3>();
        if (gameManager != null)
        {
            gameManager.KillEnemy();
        }
        Destroy(gameObject);
    }
}