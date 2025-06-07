using UnityEngine;

public class Boss : MonoBehaviour
{
    public int maxHealth = 10;
    public Animator animator;
    public Transform player;
    public float attackRange = 10f;
    private bool playerInRange = false;
    public float walkSpeed = 2f;
    public float chaseSpeed = 2.5f;
    public float retrieveDistance = 2.5f;

    public Transform detectPoint;
    public float distance;
    public LayerMask detectLayer;
    private bool facingLeft = true;

    public Transform attackPoint;
    public float attackRadius = 2f;
    public LayerMask attackLayer;

    private GameManager gameManager;
    private bool isPlayerDead = false;

    // Biến mới để kiểm soát thời gian sau khi nhận sát thương
    private bool isInDamageCooldown = false;
    private float damageCooldownDuration = 0.5f; // Thời gian animation "Boss Damage" chạy

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null) Debug.LogError("GameManager not found in the scene!");
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) Debug.LogError("Player không tìm thấy! Vui lòng đảm bảo Player có tag 'Player'.");
        animator = GetComponent<Animator>(); // Đảm bảo lấy Animator component
    }

    void Update()
    {
        if (maxHealth <= 0) Die();

        if (isPlayerDead || player == null || !player.gameObject.activeSelf)
        {
            animator.SetBool("PlayerDead", true);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerInRange = distanceToPlayer <= attackRange;
        Debug.Log($"Distance to Player: {distanceToPlayer}, PlayerInRange: {playerInRange}, Attack: {animator.GetBool("Attack")}");

        if (playerInRange)
        {
            FacePlayer();
            if (distanceToPlayer > retrieveDistance && !isInDamageCooldown)
            {
                animator.SetBool("Attack", false);
                ChasePlayer();
            }
            else if (!isInDamageCooldown)
            {
                animator.SetBool("Attack", true);
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        transform.Translate(Vector2.left * walkSpeed * Time.deltaTime);
        RaycastHit2D hit = Physics2D.Raycast(detectPoint.position, Vector2.down, distance, detectLayer);
        RaycastHit2D hitHorizontal = Physics2D.Raycast(detectPoint.position, facingLeft ? Vector2.left : Vector2.right, 0.1f, detectLayer);
        if (!hit || hitHorizontal)
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

    void FacePlayer()
    {
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

    void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collInfo && collInfo.GetComponent<Player>() != null)
        {
            collInfo.GetComponent<Player>().PlayerTakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        maxHealth -= damage;
        if (maxHealth <= 0) return;

        // Kích hoạt animation "Boss Damage"
        animator.SetBool("Damage", true);
        isInDamageCooldown = true;
        Invoke("EndDamageCooldown", damageCooldownDuration); // Kết thúc cooldown sau thời gian animation
    }

    private void EndDamageCooldown()
    {
        isInDamageCooldown = false;
        animator.SetBool("Damage", false); // Reset parameter để đảm bảo animation kết thúc
    }

    public void OnPlayerDead()
    {
        isPlayerDead = true;
        player = null;
        animator.SetBool("PlayerDead", true);
        animator.SetBool("Attack", false);
        Debug.Log($"{gameObject.name} nhận biết Player đã chết, chuyển sang trạng thái Idle.");
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
        Debug.Log(this.gameObject.name + " Die");
        if (gameManager != null)
        {
            gameManager.KillBoss(); // Thông báo Boss đã bị tiêu diệt
        }
        Destroy(this.gameObject);
    }
}