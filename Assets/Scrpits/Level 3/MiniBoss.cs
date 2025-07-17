using UnityEngine;
using System.Collections;

public class MiniBoss : MonoBehaviour
{
    public int maxHealth = 10;
    public Animator animator;
    public Transform Player_Level3;
    public float attackRange = 10f;
    private bool Player_Level3InRange = false;
    public float walkSpeed = 1.5f;
    public float jumpDistance = 5f; // Giữ lại nhưng không dùng nữa

    public Transform detectPoint;
    public float distance = 1f;
    public LayerMask detectLayer;
    private bool facingLeft = true;

    public Transform attackPoint;
    public float attackRadius = 2f;
    public LayerMask attackLayer;

    private bool isAttacking = false;
    private bool isPlayer_Level3Dead = false;

    private bool isInDamageCooldown = false;
    private float damageCooldownDuration = 0.5f;

    // Thêm biến cho summon
    public GameObject skeletonPrefab;
    private bool hasSummoned = false;

    // Biến tuần tra
    public float patrolDistance = 5f;
    private Vector2 startPosition;
    private float distanceTraveled;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator không tìm thấy trên " + gameObject.name);

        Player_Level3 = GameObject.FindGameObjectWithTag("Player_Level3")?.transform;
        if (Player_Level3 == null) Debug.LogError("Player_Level3 không tìm thấy! Vui lòng đảm bảo Player_Level3 có tag 'Player_Level3'.");

        startPosition = transform.position;
    }

    void Update()
    {
        if (maxHealth <= 0)
        {
            Die();
            return;
        }

        if (isPlayer_Level3Dead || Player_Level3 == null || !Player_Level3.gameObject.activeSelf)
        {
            animator.SetBool("PlayerDead", true);
            return;
        }

        animator.SetBool("PlayerDead", false);
        float distanceToPlayer_Level3 = Vector2.Distance(transform.position, Player_Level3.position);
        Player_Level3InRange = distanceToPlayer_Level3 <= attackRange;

        if (!Player_Level3InRange)
        {
            animator.SetBool("Attack1", false);
            animator.SetBool("Attack2", false);
            Patrol();
        }
        else
        {
            FacePlayer_Level3();
            if (!isAttacking && !isInDamageCooldown)
            {
                StartCoroutine(AttackRoutine());
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

    void FacePlayer_Level3()
    {
        if (Player_Level3 == null) return;

        if (transform.position.x < Player_Level3.position.x && facingLeft)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            facingLeft = false;
        }
        else if (transform.position.x > Player_Level3.position.x && !facingLeft)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            facingLeft = true;
        }
    }

    IEnumerator AttackRoutine()
    {
        if (isAttacking) yield break; // Ngăn lặp nếu đang tấn công
        isAttacking = true;

        // Tăng xác suất cho Attack2 (bao gồm summon)
        int randomChoice = Random.Range(0, 4); // 0-3, với 2-3 ưu tiên Attack2
        if (randomChoice < 2) // 50% cho Attack1
        {
            animator.SetBool("Attack1", true);
            animator.SetBool("Attack2", false);
        }
        else // 50% cho Attack2
        {
            animator.SetBool("Attack1", false);
            animator.SetBool("Attack2", true);
            SummonSkeleton(); // Kích hoạt summon khi dùng Attack2
        }

        yield return new WaitForSeconds(0.5f); // Chờ animation hoàn thành
        Attack();
        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
        isAttacking = false;

        // Tăng tần suất tấn công bằng cách lặp lại sau một khoảng thời gian ngắn
        yield return new WaitForSeconds(1.0f); // Chờ 1 giây trước khi tấn công lại
    }

    void ApplyJumpDamage()
    {
        // Loại bỏ vì không còn Jump, giữ lại để tương thích nhưng không sử dụng
    }

    void SummonSkeleton()
    {
        if (!hasSummoned)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-1f, 1f), -1f, 0);
                Vector3 summonPosition = transform.position + offset;
                Instantiate(skeletonPrefab, summonPosition, Quaternion.identity);
            }
            hasSummoned = true;
        }
        else
        {
            // Summon lại nếu đã hết thời gian hoặc điều kiện khác (có thể tùy chỉnh)
            for (int i = 0; i < 1; i++) // Summon ít hơn để kiểm soát số lượng
            {
                Vector3 offset = new Vector3(Random.Range(-1f, 1f), -1f, 0);
                Vector3 summonPosition = transform.position + offset;
                Instantiate(skeletonPrefab, summonPosition, Quaternion.identity);
            }
        }
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
        Player_Level3 = null;
        animator.SetBool("PlayerDead", true);
        StartCoroutine(TransitionToIdleAfterDelay(1.0f));
        Debug.Log(gameObject.name + " nhận biết Player_Level3 đã chết, chuyển sang trạng thái Idle.");
    }

    private IEnumerator TransitionToIdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("PlayerDead", false);
        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
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