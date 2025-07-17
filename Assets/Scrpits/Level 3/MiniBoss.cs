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
    private bool isPlayer_Level3Dead = false;

    private bool isInDamageCooldown = false;
    private float damageCooldownDuration = 0.5f;

    
    public float jumpHeight = 5f;
    public float jumpDuration = 1f;
    public int jumpDamage = 2;
    private bool isJumping = false;

    
    public GameObject skeletonPrefab;
    private bool hasSummoned = false;

    
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
            animator.SetBool("Attack", false);
            animator.SetBool("Attack1", false);
            animator.SetBool("Jump", false);
            Patrol();
        }
        else
        {
            FacePlayer_Level3();
            if (distanceToPlayer_Level3 > retrieveDistance && !isInDamageCooldown)
            {
                animator.SetBool("Attack", false);
                animator.SetBool("Attack1", false);
                animator.SetBool("Jump", false);
                ChasePlayer_Level3(distanceToPlayer_Level3);
            }
            else if (!isAttacking && !isJumping && !isInDamageCooldown)
            {
                StartCoroutine(AttackOrJumpRoutine());
            }
        }

       
        if (isJumping && transform.position.y <= 0.1f && !hasSummoned)
        {
            SummonSkeleton();
            ApplyJumpDamage();
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
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingLeft = false;
        }
        else if (transform.position.x > Player_Level3.position.x && !facingLeft)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingLeft = true;
        }
    }

    void ChasePlayer_Level3(float distanceToPlayer_Level3)
    {
        if (Player_Level3 == null) return;
        transform.position = Vector2.MoveTowards(transform.position, Player_Level3.position, chaseSpeed * Time.deltaTime);
    }

    IEnumerator AttackOrJumpRoutine()
    {
        isAttacking = true;
        int randomChoice = Random.Range(0, 3); // 0: Attack, 1: Attack2, 2: Jump
        if (randomChoice == 0)
        {
            animator.SetBool("Attack", true);
            animator.SetBool("Attack2", false);
            animator.SetBool("Jump", false);
        }
        else if (randomChoice == 1)
        {
            animator.SetBool("Attack", false);
            animator.SetBool("Attack2", true);
            animator.SetBool("Jump", false);
        }
        else
        {
            animator.SetBool("Attack", false);
            animator.SetBool("Attack2", false);
            animator.SetBool("Jump", true);
            yield return StartCoroutine(JumpRoutine());
        }

        yield return new WaitForSeconds(attackDelay);
        animator.SetBool("Attack", false);
        animator.SetBool("Attack1", false);
        animator.SetBool("Jump", false);
        isAttacking = false;
        isJumping = false;
    }

    IEnumerator JumpRoutine()
    {
        isJumping = true;
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y + jumpHeight, transform.position.z);

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;
            transform.position = Vector2.Lerp(targetPos, startPos, t);
            yield return null;
        }
    }

    void ApplyJumpDamage()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, attackLayer);
        foreach (Collider2D Player_Level3 in hitPlayers)
        {
            if (Player_Level3.GetComponent<Player_Level3>() != null)
            {
                Player_Level3.GetComponent<Player_Level3>().Player_Level3TakeDamage(jumpDamage);
            }
        }
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
        animator.SetBool("Attack", false);
        animator.SetBool("Attack1", false);
        animator.SetBool("Jump", false);
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