using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject vitoryUI;
    public GameObject gameOverUI;
    public int currentCoin = 0;
    public Text currentCoinText;
    public Text maxHealthText;
    public int maxHealth = 10;
    public float movement = 0f;
    public float speed = 7f;
    public Rigidbody2D rb;
    private bool facingRight = true;
    public float jumpHeight = 10f;
    private bool isGround = true;
    public Animator animator;

    public Transform attackPoint;
    public float attackRadius = 1.5f;
    public LayerMask targetLayer;

    private bool isWon = false;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    public bool hasKey = false;
    public GameObject transitionImage;
    private Animator doorAnimator;

    // Biến mới để xử lý cooldown tấn công
    private bool isAttackOnCooldown = false;
    private float attackCooldownDuration = 1f; // Thời gian cooldown (1 giây)

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager không tìm thấy trong scene!");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "Player";
            spriteRenderer.sortingOrder = 10;
            Debug.Log("Player SpriteRenderer - SortingLayer: " + spriteRenderer.sortingLayerName + ", SortingOrder: " + spriteRenderer.sortingOrder);
        }
        else
        {
            Debug.LogError("SpriteRenderer không tìm thấy trên Player! Vui lòng thêm SpriteRenderer component.");
        }

        Debug.Log("Player Start - GameObject active: " + gameObject.activeSelf + ", HP: " + maxHealth + ", Position: " + transform.position);

        GameObject door = GameObject.FindWithTag("Door");
        if (door != null)
        {
            doorAnimator = door.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Door not found in scene!");
        }

        if (vitoryUI != null)
        {
            vitoryUI.SetActive(false);
        }
        else
        {
            Debug.LogError("VictoryUI is not assigned in the Inspector!");
        }
    }

    void Update()
    {
        if (!gameObject.activeSelf || isDead)
        {
            Debug.LogError("Player is inactive or dead in Update! HP: " + maxHealth);
            return;
        }

        string spriteRendererStatus = spriteRenderer != null ? spriteRenderer.enabled.ToString() : "SpriteRenderer is null";
        Debug.Log("Player Update - GameObject active: " + gameObject.activeSelf + ", Position: " + transform.position + ", SpriteRenderer enabled: " + spriteRendererStatus);

        if (isWon)
        {
            animator.SetFloat("Walk", 0f);
            movement = 0f;
            speed = 0f;
            return;
        }

        if (maxHealth <= 0)
        {
            Die();
            return;
        }

        currentCoinText.text = currentCoin.ToString();
        maxHealthText.text = maxHealth.ToString();
        movement = Input.GetAxis("Horizontal");

        if (movement < 0f && facingRight == true)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingRight = false;
        }
        else if (movement > 0f && facingRight == false)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingRight = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true)
        {
            Jump();
            animator.SetBool("Jump", true);
            isGround = false;
        }
        if (Mathf.Abs(movement) > .1f)
        {
            animator.SetFloat("Walk", 1f);
        }
        else if (Mathf.Abs(movement) < 0.1f)
        {
            animator.SetFloat("Walk", 0f);
        }

        if (Input.GetMouseButtonDown(0) && !isAttackOnCooldown)
        {
            int randomIndex = Random.Range(0, 3);

            if (randomIndex == 0)
            {
                animator.SetTrigger("Attack1");
            }
            else if (randomIndex == 1)
            {
                animator.SetTrigger("Attack2");
            }
            else
            {
                animator.SetTrigger("Attack3");
            }

            // Kích hoạt cooldown
            isAttackOnCooldown = true;
            Invoke("EndAttackCooldown", attackCooldownDuration);
        }
    }

    private void EndAttackCooldown()
    {
        isAttackOnCooldown = false;
    }

    private void FixedUpdate()
    {
        if (gameObject.activeSelf && !isDead)
        {
            transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * speed;
        }
    }

    void Jump()
    {
        if (rb != null && gameObject.activeSelf && !isDead)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.y = jumpHeight;
            rb.linearVelocity = velocity;
        }
    }

    public void PlayerAttack()
    {
        if (gameObject.activeSelf && !isDead)
        {
            Collider2D hitInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, targetLayer);
            if (hitInfo)
            {
                Enemy enemyScript = hitInfo.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.EnemyTakeDamage(1);
                    Debug.Log("Player attacked " + hitInfo.gameObject.name + " (Enemy)!");
                }

                SpamEnemy1 spamEnemyScript = hitInfo.GetComponent<SpamEnemy1>();
                if (spamEnemyScript != null)
                {
                    spamEnemyScript.EnemyTakeDamage(1);
                    Debug.Log("Player attacked " + hitInfo.gameObject.name + " (SpamEnemy1)!");
                }

                Boss BossScript = hitInfo.GetComponent<Boss>();
                if (BossScript != null)
                {
                    BossScript.TakeDamage(1);
                    Debug.Log("Player attacked " + hitInfo.gameObject.name + " (Boss)!");
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && gameObject.activeSelf && !isDead)
        {
            isGround = true;
            animator.SetBool("Jump", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameObject.activeSelf || isDead)
        {
            return;
        }

        if (other.gameObject.tag == "Coin")
        {
            currentCoin++;
            if (gameManager != null)
            {
                gameManager.CollectItem();
            }
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Trap")
        {
            PlayerTakeDamage(2);
            Debug.Log("Player hit a trap! HP now: " + maxHealth + ", GameObject active: " + gameObject.activeSelf + ", Position: " + transform.position);
        }
        else if (other.gameObject.tag == "Key")
        {
            hasKey = true;
            Destroy(other.gameObject);
            Debug.Log("Player picked up the key! HP: " + maxHealth + ", Position: " + transform.position);
        }
        else if (other.gameObject.tag == "Door" && hasKey)
        {
            OpenDoorAndTransition();
        }
    }

    public void PlayerTakeDamage(int damage)
    {
        if (maxHealth <= 0 || !gameObject.activeSelf || isDead)
        {
            return;
        }
        maxHealth = Mathf.Max(0, maxHealth - damage);
        Debug.Log("Player took damage - HP: " + maxHealth + ", GameObject active: " + gameObject.activeSelf + ", Position: " + transform.position);

        if (maxHealth <= 0)
        {
            Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            foreach (Enemy enemy in enemies)
            {
                enemy.OnPlayerDead();
            }

            SpamEnemy1[] spamEnemies = Object.FindObjectsByType<SpamEnemy1>(FindObjectsSortMode.None);
            foreach (SpamEnemy1 spamEnemy in spamEnemies)
            {
                spamEnemy.OnPlayerDead();
            }

            Boss[] bosses = Object.FindObjectsByType<Boss>(FindObjectsSortMode.None);
            foreach (Boss boss in bosses)
            {
                boss.OnPlayerDead();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    void Die()
    {
        if (maxHealth <= 0 && gameObject.activeSelf && !isDead)
        {
            isDead = true;
            Debug.Log(this.transform.name + " Die - HP: " + maxHealth + ", GameObject active before destroy: " + gameObject.activeSelf + ", Position: " + transform.position);

            speed = 0f;
            movement = 0f;
            animator.SetFloat("Walk", 0f);
            animator.SetBool("Jump", false);

            gameObject.SetActive(false);

            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
    }

    public void OpenDoorAndTransition()
    {
        if (hasKey && gameObject.activeSelf && !isDead)
        {
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger("Open");
            }
            else
            {
                Debug.LogError("DoorAnimator is null!");
            }

            if (vitoryUI != null)
            {
                vitoryUI.SetActive(true);
                isWon = true;
                Debug.Log("Victory UI activated after reaching door with key! HP: " + maxHealth + ", Position: " + transform.position);
            }
            else
            {
                Debug.LogError("VictoryUI is null! Please assign VictoryUI in the Inspector.");
            }

            hasKey = false;
        }
    }
}