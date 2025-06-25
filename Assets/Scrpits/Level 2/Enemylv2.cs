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

    // Biến kiểm soát thời gian sau khi nhận sát thương, giống Boss
    private bool isInDamageCooldown = false;
    private float damageCooldownDuration = 0.5f; // Thời gian animation "Damage" chạy

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        player_Level2 = GameObject.FindGameObjectWithTag("Player_Level2")?.transform;
        if (player_Level2 == null) Debug.LogError("Player_Level2 không tìm thấy! Vui lòng đảm bảo Player_Level2 có tag 'Player_Level2'.");
    }

    void Update()
    {
        if (maxHealth <= 0) Die();

        if (isPlayer_Level2Dead || player_Level2 == null || !player_Level2.gameObject.activeSelf)
        {
            animator.SetBool("PlayerDead", true); // Sử dụng tham số PlayerDead từ Animator
            return;
        }

        animator.SetBool("PlayerDead", false); // Đặt lại khi player còn sống
        float distanceToPlayer_Level2 = Vector2.Distance(transform.position, player_Level2.position);
        player_Level2InRange = distanceToPlayer_Level2 <= attackRange;

        if (!player_Level2InRange)
        {
            animator.SetBool("Attack", false);
            Patrol();
        }
        else
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
        transform.Translate(Vector2.left * runSpeed * Time.deltaTime);
        RaycastHit2D hit = Physics2D.Raycast(detectPoint.position, Vector2.down, distance, detectLayer);
        if (!hit)
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
        }
    }

    void FacePlayer_Level2()
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

    void ChasePlayer_Level2(float distanceToPlayer_Level2)
    {
        transform.position = Vector2.MoveTowards(transform.position, player_Level2.position, chaseSpeed * Time.deltaTime);
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
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer); // Sửa thành OverlapCircle
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

        // Kích hoạt animation "Damage", giống Boss
        animator.SetBool("Damage", true);
        isInDamageCooldown = true;
        Invoke("EndDamageCooldown", damageCooldownDuration);
    }

    private void EndDamageCooldown()
    {
        isInDamageCooldown = false;
        animator.SetBool("Damage", false); // Reset giống Boss
    }

    public void OnPlayer_Level2Dead()
    {
        isPlayer_Level2Dead = true;
        player_Level2 = null;
        animator.SetBool("PlayerDead", true); // Kích hoạt trạng thái PlayerDead
        StartCoroutine(TransitionToIdleAfterDelay(1.0f)); // Chuyển về Idle sau delay
        StopAllCoroutines();
        Debug.Log($"{gameObject.name} nhận biết Player_Level2 đã chết, chuyển sang trạng thái Idle.");
    }

    private IEnumerator TransitionToIdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Đợi animation Player_Level2Dead hoàn tất
        animator.SetBool("PlayerDead", false); // Tắt điều kiện PlayerDead
        animator.SetBool("Attack", false); // Đảm bảo Attack bị tắt
        // Trạng thái sẽ tự động chuyển về Enemy_Idle nhờ điều kiện Exit Time
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DeathZone"))
        {
            Die();
            Debug.Log(this.gameObject.name + " fell into DeathZone!");
        }
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