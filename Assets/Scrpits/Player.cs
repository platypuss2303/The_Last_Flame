﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
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

    public GameObject vitoryUI;
    public GameObject gameOverUI;
    public int currentCoin = 0;
    public Text currentCoinText;
    public ThanhMau thanhMau; // Thêm thanh máu
    public float luongMauHienTai; // Lượng máu hiện tại
    public float luongMauToiDa = 10f; // Lượng máu tối đa
    private bool isInDamageCooldown = false; // Thêm cooldown sát thương
    private float damageCooldownDuration = 0.5f; // Thời gian cooldown sát thương
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

    private TrailRenderer tr;

    private bool isAttackOnCooldown = false;
    private float attackCooldownDuration = 1f;

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

        tr = GetComponent<TrailRenderer>();
        if (tr == null)
        {
            Debug.LogError("TrailRenderer không tìm thấy trên Player! Vui lòng thêm TrailRenderer component.");
        }

        Debug.Log("Player Start - GameObject active: " + gameObject.activeSelf + ", HP: " + luongMauHienTai + ", Position: " + transform.position);

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

        // Khởi tạo thanh máu
        luongMauHienTai = luongMauToiDa;
        if (thanhMau != null)
        {
            thanhMau.capNhatThanhMau(luongMauHienTai, luongMauToiDa);
        }
        else
        {
            Debug.LogError("ThanhMau chưa được gán trong Inspector!");
        }
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && canDash)
        {
            StartCoroutine(Dash());
        }

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Q) && canDash && !isAttackOnCooldown)
        {
            StartCoroutine(Dash());
            animator.SetTrigger("PunchTrigger");
            isAttackOnCooldown = true;
            Invoke("EndAttackCooldown", attackCooldownDuration);
            Object.FindAnyObjectByType<Sound>().PlaySound("Attack");
        }

        if (!gameObject.activeSelf || isDead)
        {
            Debug.LogError("Player is inactive or dead in Update! HP: " + luongMauHienTai);
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

        if (luongMauHienTai <= 0)
        {
            Die();
            return;
        }

        currentCoinText.text = currentCoin.ToString();
        
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

        if (Input.GetKeyDown(KeyCode.Q) && !isAttackOnCooldown && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            animator.SetTrigger("PunchTrigger");
            isAttackOnCooldown = true;
            Invoke("EndAttackCooldown", attackCooldownDuration);
            Object.FindAnyObjectByType<Sound>().PlaySound("Attack");
        }

        if (Input.GetKeyDown(KeyCode.E) && !isAttackOnCooldown)
        {
            animator.SetTrigger("PunchTrigger2");
            isAttackOnCooldown = true;
            Invoke("EndAttackCooldown", attackCooldownDuration);
            Object.FindAnyObjectByType<Sound>().PlaySound("Attack");
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

            isAttackOnCooldown = true;
            Invoke("EndAttackCooldown", attackCooldownDuration);
            Object.FindAnyObjectByType<Sound>().PlaySound("Attack");
        }
    }

    private void EndAttackCooldown()
    {
        isAttackOnCooldown = false;
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

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
            Object.FindAnyObjectByType<Sound>().PlaySound("Jump");
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
                    Object.FindAnyObjectByType<Sound>().PlaySound("Boss");
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
            Object.FindAnyObjectByType<Sound>().PlaySound("Coin");
        }
        else if (other.gameObject.tag == "Trap")
        {
            PlayerTakeDamage(2);
            Debug.Log("Player hit a trap! HP now: " + luongMauHienTai + ", GameObject active: " + gameObject.activeSelf + ", Position: " + transform.position);
        }
        else if (other.gameObject.tag == "Key")
        {
            hasKey = true;
            Destroy(other.gameObject);
            Debug.Log("Player picked up the key! HP: " + luongMauHienTai + ", Position: " + transform.position);
        }
        else if (other.gameObject.tag == "Door" && hasKey)
        {
            OpenDoorAndTransition();
        }
        else if (other.gameObject.CompareTag("DeathZone"))
        {
            PlayerTakeDamage((int)luongMauToiDa); // Gây sát thương đủ để chết
            Debug.Log("Player fell into DeathZone! HP: " + luongMauHienTai + ", Position: " + transform.position);
        }
    }

    public void PlayerTakeDamage(int damage)
    {
        if (luongMauHienTai <= 0 || !gameObject.activeSelf || isDead || isInDamageCooldown)
        {
            return;
        }
        luongMauHienTai = Mathf.Max(0, luongMauHienTai - damage);
        Debug.Log("Player took damage - HP before: " + (luongMauHienTai + damage) + ", HP after: " + luongMauHienTai + " at " + System.DateTime.Now);

        // Kích hoạt hoạt hình Damage
        animator.SetBool("Damage", true);
        isInDamageCooldown = true;
        Invoke("EndDamageCooldown", damageCooldownDuration);

        // Cập nhật thanh máu
        if (thanhMau != null)
        {
            thanhMau.capNhatThanhMau(luongMauHienTai, luongMauToiDa);
        }
        else
        {
            Debug.LogError("ThanhMau chưa được gán khi cập nhật máu!");
        }

        if (luongMauHienTai <= 0)
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

    private void EndDamageCooldown()
    {
        isInDamageCooldown = false;
        animator.SetBool("Damage", false);
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
        if (luongMauHienTai <= 0 && gameObject.activeSelf && !isDead)
        {
            isDead = true;
            Debug.Log(this.transform.name + " Die - HP: " + luongMauHienTai + ", GameObject active before destroy: " + gameObject.activeSelf + ", Position: " + transform.position);

            speed = 0f;
            movement = 0f;
            animator.SetFloat("Walk", 0f);
            animator.SetBool("Jump", false);
            animator.SetBool("Damage", false); // Đặt lại Damage khi chết

            gameObject.SetActive(false);

            if (gameManager != null)
            {
                gameManager.GameOver();
            }

            // Cập nhật thanh máu khi chết
            if (thanhMau != null)
            {
                thanhMau.capNhatThanhMau(0, luongMauToiDa);
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
                Debug.Log("Victory UI activated after reaching door with key! HP: " + luongMauHienTai + ", Position: " + transform.position);
                Time.timeScale = 0; // Dừng game khi thắng
            }
            else
            {
                Debug.LogError("VictoryUI is null! Please assign VictoryUI in the Inspector!");
            }

            hasKey = false;
        }
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