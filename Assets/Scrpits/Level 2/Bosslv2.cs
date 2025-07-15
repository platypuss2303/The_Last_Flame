using UnityEngine;
using System.Collections;

public class Bosslv2 : MonoBehaviour
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
    public float distance = 1f;
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

        player_Level2 = GameObject.FindGameObjectWithTag("Player_Level2")?.transform;
        if (player_Level2 == null)
        {
            Debug.LogError("Player_Level2 không tìm thấy! Vui lòng đảm bảo Player_Level2 có tag 'Player_Level2'.");
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

        if (isPlayer_Level2Dead || player_Level2 == null || (player_Level2 != null && !player_Level2.gameObject.activeSelf))
        {
            animator.SetBool("PlayerDead", true);
            return;
        }

        animator.SetBool("PlayerDead", false);
        if (player_Level2 != null)
        {
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
        RaycastHit2D hitHorizontal = Physics2D.Raycast(detectPoint.position, facingLeft ? Vector2.left : Vector2.right, 0.1f, detectLayer);

        if (distanceTraveled >= patrolDistance || !hit || hitHorizontal)
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

    void FacePlayer_Level2()
    {
        if (player_Level2 == null) return;

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
        if (player_Level2 == null) return;
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
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint chưa được gán trên " + gameObject.name);
            return;
        }

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

    public void OnPlayer_Level2Dead()
    {
        isPlayer_Level2Dead = true;
        player_Level2 = null;
        animator.SetBool("PlayerDead", true);
        StartCoroutine(TransitionToIdleAfterDelay(1.0f));
        Debug.Log(gameObject.name + " nhận biết Player_Level2 đã chết, chuyển sang trạng thái Idle.");
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
        Debug.Log(gameObject.name + " Died at " + System.DateTime.Now);
        
        GameManager_Level2 gameManager = FindFirstObjectByType<GameManager_Level2>();
        if (gameManager != null)
        {
            gameManager.KillBoss();
            Debug.Log(gameObject.name + " called KillBoss on GameManager_Level2 at " + System.DateTime.Now);
        }
        else
        {
            Debug.LogError("GameManager_Level2 không tìm thấy trong scene! Vui lòng đảm bảo GameManager_Level2 tồn tại.");
        }
        Destroy(gameObject);
    }
}