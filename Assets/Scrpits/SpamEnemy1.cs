using UnityEngine;

public class SpamEnemy1 : MonoBehaviour
{
    public int maxHealth = 3;
    public Animator animator;
    public Transform player;
    public float attackRange = 5f;
    private bool playerInRange = false;
    public float walkSpeed = 1.5f;
    public float chaseSpeed = 2.5f;
    public float retrieveDistance = 2f;

    public Transform detectPoint;
    public float distance;
    public LayerMask detectLayer;
    private bool facingLeft = true;
    float lastAttackTime = 0f;
    float attackChangeInterval = 1f;
    private bool firstAttack = true;

    public Transform attackPoint;
    public float attackRadius = 2f;
    public LayerMask attackLayer;

    public float moveDistance = 5f;
    private Vector2 startPosition;
    private float distanceMoved;
    private bool isPlayerDead = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) Debug.LogError("Player không tìm thấy! Vui lòng đảm bảo Player có tag 'Player'.");
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

        if (!playerInRange)
        {
            animator.SetBool("Attack", false);
            animator.SetBool("Attack2", false);
            firstAttack = true;
            Patrol();
        }
        else
        {
            FacePlayer();
            if (distanceToPlayer > retrieveDistance)
            {
                animator.SetBool("Attack", false);
                animator.SetBool("Attack2", false);
                ChasePlayer();
            }
            else
            {
                AttackPlayer();
            }
        }
    }

    void Patrol()
    {
        distanceMoved = Vector2.Distance(startPosition, transform.position);
        transform.Translate(Vector2.left * walkSpeed * Time.deltaTime);

        if (distanceMoved >= moveDistance)
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
            startPosition = transform.position;
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

    void AttackPlayer()
    {
        if (firstAttack || Time.time - lastAttackTime >= attackChangeInterval)
        {
            Debug.Log(this.gameObject.name + " is preparing to attack!");
            int randomChoice = Random.Range(0, 2);
            if (randomChoice == 0)
            {
                animator.SetBool("Attack", true);
                animator.SetBool("Attack2", false);
            }
            else
            {
                animator.SetBool("Attack", false);
                animator.SetBool("Attack2", true);
            }
            lastAttackTime = Time.time;
            firstAttack = false;
        }
    }

    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collInfo)
        {
            if (collInfo.GetComponent<Player>() != null)
            {
                collInfo.GetComponent<Player>().PlayerTakeDamage(1);
                Debug.Log(this.gameObject.name + " attacked Player!");
            }
            else if (collInfo.GetComponent<SpamEnemy1>() != null && collInfo.gameObject != this.gameObject)
            {
                collInfo.GetComponent<SpamEnemy1>().EnemyTakeDamage(1);
                Debug.Log(this.gameObject.name + " attacked " + collInfo.gameObject.name + "!");
            }
        }
    }

    public void EnemyTakeDamage(int damage)
    {
        if (maxHealth <= 0) return;
        maxHealth -= damage;
        Debug.Log(this.gameObject.name + " took damage! Current Health: " + maxHealth);
    }

    public void OnPlayerDead()
    {
        isPlayerDead = true;
        player = null;
        animator.SetBool("PlayerDead", true);
        animator.SetBool("Attack", false);
        animator.SetBool("Attack2", false);
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
        Debug.Log(this.gameObject.name + " Died");
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.KillEnemy();
        }
        Destroy(this.gameObject);
    }
}