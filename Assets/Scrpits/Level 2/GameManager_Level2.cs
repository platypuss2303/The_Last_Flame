using UnityEngine;
using System.Collections;

public class GameManager_Level2 : MonoBehaviour
{
    public int totalItems = 6;    // Số lượng vật phẩm cần thu thập
    public int totalEnemies = 2;  // Số lượng quái cần tiêu diệt
    private int collectedItems = 0;
    private int enemiesKilled = 0;
    private bool isBossKilled = false; // Trạng thái boss bị tiêu diệt

    // Tham chiếu đến các GameObject cha
    public GameObject seaParent;    // Gán sea_0
    public GameObject skyParent;    // Gán sky_0
    public GameObject cloudParent;  // Gán clouds_0

    // Màu sắc
    public Color cursedColor = new Color(0.2f, 0.4f, 0.5f);  // Màu u ám ban đầu
    public Color cleanWaterColor = new Color(0f, 0.6f, 0.8f); // Màu nước trong
    public Color dayColor = new Color(1f, 0.95f, 0.8f);      // Màu ban ngày

    public GameObject gameOverUI; // UI khi game over
    public GameObject victoryUI;  // UI khi thắng

    private SpriteRenderer[] seaRenderers;    // Lưu trữ các SpriteRenderer con của sea
    private SpriteRenderer[] skyRenderers;    // Lưu trữ các SpriteRenderer con của sky
    private SpriteRenderer[] cloudRenderers;  // Lưu trữ các SpriteRenderer con của cloud

    private bool hasChangedColor = false;
    private bool hasWaterCleaned = false;

    void Start()
    {
        // Lấy tất cả SpriteRenderer từ các GameObject con
        if (seaParent != null)
            seaRenderers = seaParent.GetComponentsInChildren<SpriteRenderer>();
        if (skyParent != null)
            skyRenderers = skyParent.GetComponentsInChildren<SpriteRenderer>();
        if (cloudParent != null)
            cloudRenderers = cloudParent.GetComponentsInChildren<SpriteRenderer>();

        // Đặt màu ban đầu
        SetInitialColors();
        StartCoroutine(FadeInCursedEffect());
    }

    void SetInitialColors()
    {
        if (seaRenderers != null)
            foreach (SpriteRenderer sr in seaRenderers)
                if (sr != null) sr.color = cursedColor;
        if (skyRenderers != null)
            foreach (SpriteRenderer sr in skyRenderers)
                if (sr != null) sr.color = cursedColor;
        if (cloudRenderers != null)
            foreach (SpriteRenderer cr in cloudRenderers)
                if (cr != null) cr.color = cursedColor;
    }

    private IEnumerator FadeInCursedEffect()
    {
        float duration = 2.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            if (seaRenderers != null)
                foreach (SpriteRenderer sr in seaRenderers)
                    if (sr != null) sr.color = Color.Lerp(Color.white, cursedColor, t);
            if (skyRenderers != null)
                foreach (SpriteRenderer sr in skyRenderers)
                    if (sr != null) sr.color = Color.Lerp(Color.white, cursedColor, t);
            if (cloudRenderers != null)
                foreach (SpriteRenderer cr in cloudRenderers)
                    if (cr != null) cr.color = Color.Lerp(Color.white, cursedColor, t);
            yield return null;
        }
    }

    void Update()
    {
        if (!hasChangedColor && !hasWaterCleaned && collectedItems >= totalItems && enemiesKilled >= totalEnemies && isBossKilled)
        {
            StartCoroutine(ChangeToDayMode());
            hasChangedColor = true;
            hasWaterCleaned = true;
        }
    }

    public void CollectItem() { collectedItems++; }
    public void KillEnemy() { enemiesKilled++; }
    public void KillBoss() { isBossKilled = true; }

    public bool IsBossKilled() { return isBossKilled; }

    public void GameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            Time.timeScale = 1f; // Giữ game chạy
            Debug.Log("Game Over UI activated at " + System.DateTime.Now);
        }
        else
        {
            Debug.LogError("gameOverUI is not assigned in the Inspector!");
        }
    }

    public void Victory()
    {
        if (victoryUI != null)
        {
            victoryUI.SetActive(true);
            Debug.Log("Victory UI activated at " + System.DateTime.Now);
        }
        else
        {
            Debug.LogError("victoryUI is not assigned in the Inspector!");
        }
    }

    private IEnumerator ChangeToDayMode()
    {
        float duration = 2.0f;
        float elapsedTime = 0f;

        // Đổi màu nước trước
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            if (seaRenderers != null)
                foreach (SpriteRenderer sr in seaRenderers)
                    if (sr != null) sr.color = Color.Lerp(cursedColor, cleanWaterColor, t);
            yield return null;
        }

        // Đổi màu toàn cảnh sau
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            if (skyRenderers != null)
                foreach (SpriteRenderer sr in skyRenderers)
                    if (sr != null) sr.color = Color.Lerp(cursedColor, dayColor, t);
            if (cloudRenderers != null)
                foreach (SpriteRenderer cr in cloudRenderers)
                    if (cr != null) cr.color = Color.Lerp(cursedColor, dayColor, t);
            yield return null;
        }
    }
}