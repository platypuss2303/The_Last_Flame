using UnityEngine;
using System.Collections;

public class Batlv3 : MonoBehaviour
{
    public float flySpeed = 2.5f;
    public float chaseSpeed = 3.5f;
    public float attackRange = 10f;
    public float attackRadius = 2f;
    public Transform player_Level3;
    public Transform attackPoint;
    public LayerMask attackLayer;
    public Transform startingPoint;
    public int maxHealth = 3;
    [HideInInspector] public bool facingLeft = false;
    private Animator animator;
    private bool isAttacking = false;
    public float attackDelay = 1.5f;
    private bool isInDamageCooldown = false;
    private float damageCooldownDuration = 0.5f;
    public bool chase = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        player_Level3 = GameObject.FindGameObjectWithTag("Player_Level3")?.transform;
        if (player_Level3 == null) Debug.LogError("Player_Level3 không tìm thấy!");
        if (startingPoint == null) startingPoint = transform; // Dùng vị trí ban đầu nếu không gán
        if (attackPoint == null) Debug.LogError("attackPoint không được gán!");
    }

    void Update()
    {
        if (player_Level3 == null || maxHealth <= 0) return;

        if (chase)
        {
            Chase();
        }
        else
        {
            ReturnStartPoint();
        }

        // Xử lý tấn công nếu trong phạm vi
        if (chase && Vector2.Distance(transform.position, player_Level3.position) <= attackRadius && !isAttacking && !isInDamageCooldown)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    void Chase()
    {
        float distanceToPlayer_Level3 = Vector2.Distance(transform.position, player_Level3.position);
        if (distanceToPlayer_Level3 > attackRadius && !isAttacking && !isInDamageCooldown)
        {
            Vector2 direction = (player_Level3.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player_Level3.position, chaseSpeed * Time.deltaTime);
            UpdateFacingDirection(direction.x); // Cập nhật hướng ngay lập tức
            Debug.Log($"Chase: Direction X = {direction.x}, Facing Left = {facingLeft}, Distance = {distanceToPlayer_Level3}");
            animator.SetBool("Attack", false);
        }
    }

    void ReturnStartPoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, startingPoint.position, flySpeed * Time.deltaTime);
        UpdateFacingDirection((startingPoint.position.x - transform.position.x)); // Cập nhật hướng về startingPoint
        Debug.Log($"Return: Direction X = {(startingPoint.position.x - transform.position.x)}, Facing Left = {facingLeft}");
        animator.SetBool("Attack", false);
    }

    void UpdateFacingDirection(float moveDirection)
    {
        if (moveDirection > 0.01f && facingLeft)
        {
            transform.localScale = new Vector3(1, 1, 1); // Mặt phải
            facingLeft = false;
            UpdateAttackPointPosition();
            Debug.Log("Turned Right");
        }
        else if (moveDirection < -0.01f && !facingLeft) // Di chuyển sang trái
        {
            transform.localScale = new Vector3(-1, 1, 1); // Mặt trái
            facingLeft = true;
            UpdateAttackPointPosition();
            Debug.Log("Turned Left");
        }
    }

    void UpdateAttackPointPosition()
    {
        // Đảm bảo attackPoint luôn ở phía trước mặt dơi
        if (attackPoint != null)
        {
            Vector3 localPos = attackPoint.localPosition;
            float xPos = Mathf.Abs(localPos.x) * (facingLeft ? -1 : 1); // Lật xPos dựa trên facingLeft
            attackPoint.localPosition = new Vector3(xPos, localPos.y, localPos.z);
            Debug.Log($"AttackPoint updated to X = {xPos}, Facing Left = {facingLeft}");
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("Attack", true);
        UpdateFacingDirection((player_Level3.position.x - transform.position.x)); // Quay mặt về player ngay lập tức
        Debug.Log($"Attacking: Direction X = {(player_Level3.position.x - transform.position.x)}, Facing Left = {facingLeft}");
        yield return new WaitForSeconds(attackDelay);
        animator.SetBool("Attack", false);
        isAttacking = false;
        Attack();
    }

    public void Attack()
    {
        if (attackPoint == null) return;
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collInfo && collInfo.GetComponent<Player_Level3>() != null)
        {
            collInfo.GetComponent<Player_Level3>().Player_Level3TakeDamage(1);
            Debug.Log("Attacked Player!");
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

        if (maxHealth <= 0)
        {
            Die();
        }
    }

    private void EndDamageCooldown()
    {
        isInDamageCooldown = false;
        animator.SetBool("Damage", false);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
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