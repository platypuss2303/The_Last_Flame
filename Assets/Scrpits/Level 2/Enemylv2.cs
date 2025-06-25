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
<<<<<<< Updated upstream
    private bool isPlayerDead = false;
=======
    private bool isPlayer_Level2Dead = false;

    private bool isInDamageCooldown = false;
    private float damageCooldownDuration = 0.5f;
>>>>>>> Stashed changes

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
<<<<<<< Updated upstream
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) Debug.LogError("Player không tìm thấy! Vui lòng đảm bảo Player có tag 'Player'.");
=======
        player_Level2 = GameObject.FindGameObjectWithTag("Player_Level2")?.transform;
        if (player_Level2 == null) Debug.LogError("Player_Level2 không tìm thấy! Vui lòng đảm bảo Player_Level2 có tag 'Player_Level2'. at " + System.DateTime.Now);
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            collInfo.GetComponent<Player>().PlayerTakeDamage(1);
=======
            Player_Level2 player = collInfo.GetComponent<Player_Level2>();
            player.Player_Level2TakeDamage(1);
            if (player.maxHealth <= 0 && !isPlayer_Level2Dead)
            {
                isPlayer_Level2Dead = true;
                player.OnPlayer_Level2Dead(); // Thông báo khi Player chết
            }
>>>>>>> Stashed changes
        }
    }

    public void EnemyTakeDamage(int damage)
    {
        if (maxHealth <= 0) return;
        maxHealth -= damage;
<<<<<<< Updated upstream
=======
        Debug.Log(this.gameObject.name + " nhận sát thương, HP còn: " + maxHealth + " at " + System.DateTime.Now);

        animator.SetBool("Damage", true);
        isInDamageCooldown = true;
        Invoke("EndDamageCooldown", damageCooldownDuration);
>>>>>>> Stashed changes
    }

    public void OnPlayerDead()
    {
<<<<<<< Updated upstream
        isPlayerDead = true;
        player = null;
        animator.SetBool("PlayerDead", true);
        animator.SetBool("Attack", false);
        StopAllCoroutines();
        Debug.Log($"{gameObject.name} nhận biết Player đã chết, chuyển sang trạng thái Idle.");
=======
        isInDamageCooldown = false;
        animator.SetBool("Damage", false);
    }

    public void OnPlayer_Level2Dead()
    {
        isPlayer_Level2Dead = true;
        player_Level2 = null;
        animator.SetBool("Player_Level2Dead", true);
        animator.SetBool("Attack", false);
        StopAllCoroutines();
        Debug.Log($"{gameObject.name} nhận biết Player_Level2 đã chết, chuyển sang trạng thái Idle. at " + System.DateTime.Now);
>>>>>>> Stashed changes
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DeathZone"))
        {
            Die();
            Debug.Log(this.gameObject.name + " fell into DeathZone! at " + System.DateTime.Now);
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
        Debug.Log(this.gameObject.name + " Died at " + System.DateTime.Now);
        GameManager_Level2 gameManager = FindFirstObjectByType<GameManager_Level2>();
        if (gameManager != null)
        {
            gameManager.KillEnemy();
            Debug.Log("KillEnemy() called on GameManager_Level2 at " + System.DateTime.Now);
        }
        else
        {
            Debug.LogError("GameManager_Level2 not found in scene! at " + System.DateTime.Now);
        }
        gameObject.SetActive(false); // Thay Destroy bằng SetActive(false)
    }
}