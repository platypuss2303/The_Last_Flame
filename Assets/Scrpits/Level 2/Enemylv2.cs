using UnityEngine;
using System.Collections;

public class EnemyLv2 : MonoBehaviour
{
    public int maxHealth = 3;
    public Animator animator;
    public Transform player;
    public float attackRange = 10f;
    private bool playerInRange = false;
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
    private bool isPlayerDead = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
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
            Patrol();
        }
        else
        {
            FacePlayer();
            if (distanceToPlayer > retrieveDistance)
            {
                animator.SetBool("Attack", false);
                ChasePlayer(distanceToPlayer);
            }
            else if (!isAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
        }
    }

    void Patrol()
    {
        transform.Translate(Vector2.left * runSpeed * Time.deltaTime); // Sử dụng runSpeed thay vì walkSpeed
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

    void ChasePlayer(float distanceToPlayer)
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        animator.SetBool("Attack", true); // Chỉ dùng 1 attack

        yield return new WaitForSeconds(attackDelay);

        animator.SetBool("Attack", false);
        isAttacking = false;
    }

    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collInfo && collInfo.GetComponent<Player>() != null)
        {
            collInfo.GetComponent<Player>().PlayerTakeDamage(1);
        }
    }

    public void EnemyTakeDamage(int damage)
    {
        if (maxHealth <= 0) return;
        maxHealth -= damage;
    }

    public void OnPlayerDead()
    {
        isPlayerDead = true;
        player = null;
        animator.SetBool("PlayerDead", true);
        animator.SetBool("Attack", false);
        StopAllCoroutines();
        Debug.Log($"{gameObject.name} nhận biết Player đã chết, chuyển sang trạng thái Idle.");
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