using UnityEngine;
using System.Collections;

public class EnemyLv2 : MonoBehaviour
{
    public int maxHealth = 3;
    public Animator animator;
    public Transform player_Level2;
    public float attackRange = 10f;
    private bool player_Level2InRange = false;
    public float runSpeed = 2.5f;
    public float chaseSpeed = 3.5f;
    public float retrieveDistance = 2.5f;

    public Transform detectPoint;
    public float distance;
    public LayerMask detectLayer;
    private bool facingLeft = true;

    public Transform attackPoint;
    public float attackRadius = 2f;
    public LayerMask attackLayer;

    private bool isAttacking = false;
    public float attackDelay = 1.5f;
    private bool isPlayer_Level2Dead = false;

    private bool isInDamageCooldown = false;
    private float damageCooldownDuration = 0.5f;

    private Rigidbody2D rb;
    private bool isTurning = false; // Thêm flag để tạm dừng di chuyển khi quay

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D không được gắn vào EnemyLv2!");
            return;
        }
        if (animator == null) animator = GetComponent<Animator>();
        player_Level2 = GameObject.FindGameObjectWithTag("Player_Level2")?.transform;
        if (player_Level2 == null) Debug.LogError("Player_Level2 không tìm thấy!");
    }

    void FixedUpdate()
    {
        if (maxHealth <= 0) Die();

        if (isPlayer_Level2Dead || player_Level2 == null || !player_Level2.gameObject.activeSelf)
        {
            animator.SetBool("PlayerDead", true);
            return;
        }

        animator.SetBool("PlayerDead", false);
        float distanceToPlayer_Level2 = Vector2.Distance(transform.position, player_Level2.position);
        player_Level2InRange = distanceToPlayer_Level2 <= attackRange;

        if (!player_Level2InRange && !isTurning)
        {
            animator.SetBool("Attack", false);
            Patrol();
        }
        else if (!isTurning)
        {
            FacePlayer_Level2();
            if (distanceToPlayer_Level2 > retrieveDistance && !isInDamageCooldown)
            {
                animator.SetBool("Attack", false);
                ChasePlayer_Level2(distanceToPlayer_Level2);
            }
            else if (!isAttacking && !isInDamageCooldown)
            {
                StartCoroutine(AttackRoutine());
            }
        }
    }

    void Patrol()
    {
        if (!isTurning)
        {
            Vector2 moveDirection = facingLeft ? Vector2.left : Vector2.right;
            rb.MovePosition(rb.position + moveDirection * runSpeed * Time.fixedDeltaTime);
            RaycastHit2D hit = Physics2D.Raycast(detectPoint.position, Vector2.down, distance, detectLayer);
            if (!hit)
            {
                StartTurning();
            }
        }
    }

    void FacePlayer_Level2()
    {
        if (!isTurning)
        {
            if (transform.position.x < player_Level2.position.x && facingLeft)
            {
                transform.eulerAngles = new Vector3(0f, -180f, 0f);
                facingLeft = false;
            }
            else if (transform.position.x > player_Level2.position.x && !facingLeft)
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                facingLeft = true;
            }
        }
    }

    void ChasePlayer_Level2(float distanceToPlayer_Level2)
    {
        if (!isTurning && player_Level2 != null)
        {
            Vector2 targetPosition = Vector2.MoveTowards(rb.position, player_Level2.position, chaseSpeed * Time.fixedDeltaTime);
            rb.MovePosition(targetPosition);
        }
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
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collInfo && collInfo.GetComponent<Player_Level2>() != null)
        {
            collInfo.GetComponent<Player_Level2>().Player_Level2TakeDamage(1);
        }
    }

    public void EnemyTakeDamage(int damage)
    {
        if (maxHealth <= 0 || isInDamageCooldown) return;
        maxHealth -= damage;
        Debug.Log(this.gameObject.name + " nhận sát thương, HP còn: " + maxHealth);
        animator.SetBool("Damage", true);
        isInDamageCooldown = true;
        Invoke("EndDamageCooldown", damageCooldownDuration);
    }

    private void EndDamageCooldown()
    {
        isInDamageCooldown = false;
        animator.SetBool("Damage", false);
    }

    public void OnPlayer_Level2Dead()
    {
        isPlayer_Level2Dead = true;
        player_Level2 = null;
        animator.SetBool("PlayerDead", true);
        StartCoroutine(TransitionToIdleAfterDelay(1.0f));
        StopAllCoroutines();
        Debug.Log($"{gameObject.name} nhận biết Player_Level2 đã chết.");
    }

    private IEnumerator TransitionToIdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("PlayerDead", false);
        animator.SetBool("Attack", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DeathZone"))
        {
            Die();
            Debug.Log(this.gameObject.name + " fell into DeathZone!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Va chạm với {collision.gameObject.name}, tag: {collision.gameObject.tag}, contact count: {collision.contactCount}");
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Mush"))
        {
            StartTurning(); // Gọi hàm quay mặt giống Patrol
            Vector2 awayDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(awayDirection * 50f); // Tách ra nhưng vẫn quay
        }
    }

    private void StartTurning()
    {
        isTurning = true;
        facingLeft = !facingLeft;
        transform.eulerAngles = new Vector3(0, facingLeft ? -180 : 0, 0);
        Debug.Log($"{gameObject.name} đang quay mặt tại {transform.position}");
        StartCoroutine(EndTurning(0.1f)); // Tạm dừng di chuyển trong 0.1s
    }

    private IEnumerator EndTurning(float delay)
    {
        yield return new WaitForSeconds(delay);
        isTurning = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (detectPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(detectPoint.position, Vector2.down * distance);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }

    void Die()
    {
        Debug.Log(this.gameObject.name + " Died");
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.KillEnemy();
        }
        Destroy(this.gameObject);
    }
}