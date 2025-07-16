using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player_Level3 : MonoBehaviour
{
    public GameObject ghostEffect;
    public float ghostDelaySeconds = 0.05f;
    public float ghostLifetime = 0.5f;
    private Coroutine dashEffectCoroutine;

    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 10f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    public GameObject victoryUI;
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
    private GameManager_Level3 gameManager;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    public bool hasKey = false;
    public GameObject transitionImage;
    private Animator doorAnimator;

    private TrailRenderer tr;

    private bool isAttackOnCooldown = false;
    private float attackCooldownDuration = 1f;

    private Sound sound;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager_Level3>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager_Level3 không tìm th?y trong scene! Vui lòng thêm GameManager_Level3 vào scene. at " + System.DateTime.Now);
        }
        else
        {
            Debug.Log("GameManager_Level3 found at " + System.DateTime.Now);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "Player";
            spriteRenderer.sortingOrder = 10;
        }
        else Debug.LogError("SpriteRenderer không tìm th?y trên Player!");

        tr = GetComponent<TrailRenderer>();
        if (tr == null) Debug.LogError("TrailRenderer không tìm th?y trên Player!");

        GameObject door = GameObject.FindWithTag("Door");
        if (door != null) doorAnimator = door.GetComponent<Animator>();
        else Debug.LogError("Door not found in scene!");

        if (victoryUI != null) victoryUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);

        sound = FindAnyObjectByType<Sound>();
        if (sound == null)
        {
            Debug.LogError("Sound object không tìm th?y trong scene! Vui lòng thêm GameObject v?i script Sound.");
        }
    }

    void Update()
    {
        if (isDashing) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q) && canDash && !isAttackOnCooldown)
        {
            StartCoroutine(Dash());
            animator.SetTrigger("PunchTrigger");
            isAttackOnCooldown = true;
            Invoke("EndAttackCooldown", attackCooldownDuration);
            if (sound != null) sound.PlaySound("Attack");
            else Debug.LogWarning("Sound object không tìm th?y khi phát Attack sound!");
        }
        if (!gameObject.activeSelf || isDead) return;

        if (isWon)
        {
            animator.SetFloat("Walk", 0f);
            movement = 0f;
            speed = 0f;
            return;
        }

        currentCoinText.text = currentCoin.ToString();
        maxHealthText.text = maxHealth.ToString();
        movement = Input.GetAxis("Horizontal");

        if (movement < 0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingRight = false;
        }
        else if (movement > 0f && !facingRight)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingRight = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
            animator.SetBool("Jump", true);
            isGround = false;
            if (sound != null) sound.PlaySound("Jump");
            else Debug.LogWarning("Sound object không tìm th?y khi phát Jump sound!");
        }

        if (Mathf.Abs(movement) > 0.1f)
        {
            animator.SetFloat("Walk", 1f);
        }
        else
        {
            animator.SetFloat("Walk", 0f);
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isAttackOnCooldown)
        {
            animator.SetTrigger("PunchTrigger");
            isAttackOnCooldown = true;
            Invoke("EndAttackCooldown", attackCooldownDuration);
            if (sound != null) sound.PlaySound("Attack");
            else Debug.LogWarning("Sound object không tìm th?y khi phát Attack sound!");
        }

        if (Input.GetKeyDown(KeyCode.E) && !isAttackOnCooldown)
        {
            animator.SetTrigger("PunchTrigger2");
            isAttackOnCooldown = true;
            Invoke("EndAttackCooldown", attackCooldownDuration);
            if (sound != null) sound.PlaySound("Attack");
            else Debug.LogWarning("Sound object không tìm th?y khi phát Attack sound!");
        }

        if (Input.GetMouseButtonDown(0) && !isAttackOnCooldown)
        {
            int randomIndex = Random.Range(0, 3);
            if (randomIndex == 0) animator.SetTrigger("Attack1");
            else if (randomIndex == 1) animator.SetTrigger("Attack2");
            else animator.SetTrigger("Attack3");
            isAttackOnCooldown = true;
            Invoke("EndAttackCooldown", attackCooldownDuration);
            if (sound != null) sound.PlaySound("Attack");
            else Debug.LogWarning("Sound object không tìm th?y khi phát Attack sound!");
        }
    }

    private void EndAttackCooldown()
    {
        isAttackOnCooldown = false;
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        if (gameObject.activeSelf && !isDead)
        {
            transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * speed;
        }
    }

    void Jump()
    {
        if (rb != null && gameObject.activeSelf && !isDead)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        }
    }

    public void PlayerAttack()
    {
        if (gameObject.activeSelf && !isDead)
        {
            Collider2D hitInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, targetLayer);
            if (hitInfo)
            {
                MonoBehaviour enemy = hitInfo.GetComponent<MonoBehaviour>();
                if (enemy != null)
                {
                    System.Reflection.MethodInfo method = enemy.GetType().GetMethod("EnemyTakeDamage");
                    if (method != null)
                    {
                        method.Invoke(enemy, new object[] { 1 });
                        Debug.Log("Player attacked " + hitInfo.gameObject.name + " with EnemyTakeDamage! at " + System.DateTime.Now);
                        if (hitInfo.CompareTag("BossLv3"))
                        {
                            if (sound != null) sound.PlaySound("Boss");
                            else Debug.LogWarning("Sound object không tìm th?y khi phát Boss sound!");
                        }
                        else
                        {
                            if (sound != null) sound.PlaySound("Attack");
                            else Debug.LogWarning("Sound object không tìm th?y khi phát Attack sound!");
                        }
                    }
                }
            }
            else
            {
                Debug.Log("No enemy detected at attackPoint: " + attackPoint.position + " with radius " + attackRadius + " at " + System.DateTime.Now);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && !isDead)
        {
            isGround = true;
            animator.SetBool("Jump", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameObject.activeSelf || isDead) return;

        if (other.gameObject.tag == "Coin")
        {
            currentCoin++;
            if (gameManager != null) gameManager.CollectItem();
            Destroy(other.gameObject);
            if (sound != null) sound.PlaySound("Coin");
            else Debug.LogWarning("Sound object không tìm th?y khi phát Coin sound!");
        }
        else if (other.gameObject.tag == "Key")
        {
            if (gameManager != null)
            {
                gameManager.CollectKey();
                Debug.Log("Player_Level3 picked up the key and called GameManager_Level3.CollectKey at " + System.DateTime.Now);
            }
            else
            {
                Debug.LogError("GameManager_Level3 not found when collecting key!");
            }
            Destroy(other.gameObject);
            if (sound != null) sound.PlaySound("Key");
            else Debug.LogWarning("Sound object không tìm th?y khi phát Key sound!");
        }
        else if (other.gameObject.tag == "Door")
        {
            if (gameManager != null)
            {
                if (doorAnimator != null) doorAnimator.SetTrigger("Open");
                gameManager.OnPlayerReachDoor();
                Debug.Log("Player_Level3 reached door and called GameManager_Level3.OnPlayerReachDoor at " + System.DateTime.Now);
                if (sound != null) sound.PlaySound("Door");
                else Debug.LogWarning("Sound object không tìm th?y khi phát Door sound!");
            }
            else
            {
                Debug.LogError("GameManager_Level3 not found when reaching door!");
            }
        }
        else if (other.gameObject.CompareTag("DeathZone"))
        {
            Debug.Log("Entered DeathZone at " + System.DateTime.Now);
            Die();
            if (sound != null) sound.PlaySound("Death");
            else Debug.LogWarning("Sound object không tìm th?y khi phát Death sound!");
        }
    }

    public void Player_Level3TakeDamage(int damage)
    {
        if (maxHealth <= 0 || !gameObject.activeSelf || isDead) return;
        maxHealth = Mathf.Max(0, maxHealth - damage);
        Debug.Log("Player took damage - HP before: " + (maxHealth + damage) + ", HP after: " + maxHealth + " at " + System.DateTime.Now);
        maxHealthText.text = maxHealth.ToString();

        if (maxHealth == 0 && !isDead)
        {
            Debug.Log("Health reached 0, calling Die() at " + System.DateTime.Now);
            Die();
        }
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
        else if (gameObject.activeSelf && !isDead)
        {
            isDead = true;
            maxHealth = 0;
            Debug.Log(this.transform.name + " Die from falling - HP: " + maxHealth + ", Position: " + transform.position);

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

    public void ResetDeadState()
    {
        isDead = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        float dashDirection = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashingPower, 0f);
        tr.emitting = true;
        StartDashEffect();
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        StopDashEffect();
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    void StopDashEffect()
    {
        if (dashEffectCoroutine != null)
        {
            StopCoroutine(dashEffectCoroutine);
            dashEffectCoroutine = null;
        }
    }

    void StartDashEffect()
    {
        if (dashEffectCoroutine != null) StopCoroutine(dashEffectCoroutine);
        dashEffectCoroutine = StartCoroutine(DashEffectCoroutine());
    }

    IEnumerator DashEffectCoroutine()
    {
        while (isDashing)
        {
            if (ghostEffect != null && spriteRenderer != null)
            {
                GameObject ghost = Instantiate(ghostEffect, transform.position, transform.rotation);
                SpriteRenderer ghostSR = ghost.GetComponent<SpriteRenderer>();
                if (ghostSR != null)
                {
                    ghostSR.sprite = spriteRenderer.sprite;
                    ghostSR.flipX = !facingRight;
                    ghostSR.sortingLayerName = "Player";
                    ghostSR.sortingOrder = 9;
                    Color ghostColor = ghostSR.color;
                    ghostColor.a = 0.5f;
                    ghostSR.color = ghostColor;
                }
                Destroy(ghost, ghostLifetime);
            }
            yield return new WaitForSeconds(ghostDelaySeconds);
        }
    }
}