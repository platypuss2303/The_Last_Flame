using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_Level3 : MonoBehaviour
{
    public int totalItems = 6;
    public int totalEnemies = 2;
    private int collectedItems = 0;
    private int enemiesKilled = 0;
    private bool isBossKilled = false; // Theo dõi boss bị tiêu diệt (1 boss)
    private bool hasKeyCollected = false; // Theo dõi người chơi đã nhặt chìa khóa chưa

    public GameObject skyParent;
    public GameObject cloudParent;
    public GameObject key; // Biến cho chìa khóa (gắn script Key_Level3)

    public Color cursedColor = new Color(0.2f, 0.4f, 0.5f);
    public Color dayColor = new Color(1f, 0.95f, 0.8f);

    public GameObject gameOverScreen;
    public GameObject victoryUI;

    private SpriteRenderer[] skyRenderers;
    private SpriteRenderer[] cloudRenderers;

    private bool hasChangedColor = false;
    private bool isGameOver = false;

    void Start()
    {
        if (skyParent != null) skyRenderers = skyParent.GetComponentsInChildren<SpriteRenderer>();
        else Debug.LogWarning("skyParent chưa được gán trong GameManager_Level3!");

        if (cloudParent != null) cloudRenderers = cloudParent.GetComponentsInChildren<SpriteRenderer>();
        else Debug.LogWarning("cloudParent chưa được gán trong GameManager_Level3!");

        // Đặt màu ban đầu và chạy hiệu ứng fade
        SetInitialColors();
        StartCoroutine(FadeInCursedEffect());
    }

    void SetInitialColors()
    {
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
            if (skyRenderers != null)
                foreach (SpriteRenderer sr in skyRenderers)
                    if (sr != null) sr.color = Color.Lerp(Color.white, cursedColor, t);
            if (cloudRenderers != null)
                foreach (SpriteRenderer cr in cloudRenderers)
                    if (cr != null) cr.color = Color.Lerp(Color.white, cursedColor, t);
            yield return null;
        }

        SetInitialColors();
    }

    void Update()
    {
        // Đổi màu và hiện chìa khóa khi boss bị tiêu diệt
        if (!hasChangedColor && isBossKilled)
        {
            Debug.Log("Boss đã bị tiêu diệt - Đổi sang màu bình thường và hiện chìa khóa tại " + System.DateTime.Now);
            StartCoroutine(ChangeToNormalColors());
            ShowKey();
            hasChangedColor = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void CollectItem()
    {
        collectedItems++;
        Debug.Log($"Đã thu thập vật phẩm: {collectedItems}/{totalItems} tại " + System.DateTime.Now);
    }

    public void KillEnemy()
    {
        enemiesKilled++;
        Debug.Log($"Đã tiêu diệt kẻ thù: {enemiesKilled}/{totalEnemies} tại " + System.DateTime.Now);
    }

    public void KillBoss()
    {
        isBossKilled = true;
        Debug.Log($"Boss đã bị tiêu diệt: isBossKilled={isBossKilled} tại " + System.DateTime.Now);
    }

    public bool IsBossKilled()
    {
        return isBossKilled;
    }

    public void GameOver()
    {
        if (isGameOver) return;

        Debug.Log("Game Over! Người chơi đã bị hạ gục tại " + System.DateTime.Now);
        isGameOver = true;

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            Time.timeScale = 1f;
            Debug.Log("Màn hình Game Over được kích hoạt tại " + System.DateTime.Now);
        }
        else
        {
            Debug.LogWarning("GameOverScreen chưa được gán trong GameManager_Level3!");
        }
    }

    public void Victory()
    {
        if (victoryUI != null)
        {
            victoryUI.SetActive(true);
            Time.timeScale = 0; // Dừng game khi thắng, giống Level 1
            Debug.Log("Màn hình Victory được kích hoạt tại " + System.DateTime.Now);
        }
        else
        {
            Debug.LogError("victoryUI chưa được gán trong GameManager_Level3!");
        }
    }

    private IEnumerator ChangeToNormalColors()
    {
        float duration = 2.0f;
        float elapsedTime = 0f;

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

        if (skyRenderers != null)
            foreach (SpriteRenderer sr in skyRenderers)
                if (sr != null) sr.color = dayColor;
        if (cloudRenderers != null)
            foreach (SpriteRenderer cr in cloudRenderers)
                if (cr != null) cr.color = dayColor;

        Debug.Log("Đã đổi sang màu bình thường (dayColor cho trời và mây) tại " + System.DateTime.Now);
    }

    private void ShowKey()
    {
        if (key == null)
        {
            Debug.LogWarning("Chìa khóa chưa được gán trong GameManager_Level3! tại " + System.DateTime.Now);
            return;
        }
        key.SetActive(true);
        SpriteRenderer keyRenderer = key.GetComponent<SpriteRenderer>();
        if (keyRenderer == null)
        {
            Debug.LogWarning("Chìa khóa không có SpriteRenderer! tại " + System.DateTime.Now);
            return;
        }
        keyRenderer.sortingLayerName = "Player";
        keyRenderer.sortingOrder = 20;
        keyRenderer.enabled = true;
        keyRenderer.color = Color.yellow;
        Debug.Log($"Chìa khóa xuất hiện - Active: {key.activeSelf}, Enabled: {keyRenderer.enabled}, Sprite: {(keyRenderer.sprite != null ? keyRenderer.sprite.name : "None")}, Vị trí: {key.transform.position}, Màu: {keyRenderer.color} tại " + System.DateTime.Now);
    }

    public void CollectKey()
    {
        hasKeyCollected = true;
        Debug.Log("Người chơi đã nhặt chìa khóa tại " + System.DateTime.Now);
    }

    public void OnPlayerReachDoor()
    {
        if (hasKeyCollected)
        {
            Victory();
            Debug.Log("Người chơi đến cửa với chìa khóa, kích hoạt Victory tại " + System.DateTime.Now);
        }
        else
        {
            Debug.Log("Người chơi đến cửa nhưng chưa có chìa khóa tại " + System.DateTime.Now);
        }
    }

    public void Pause()
    {
        if (UIController.Instance != null && UIController.Instance.pausePanel != null)
        {
            if (UIController.Instance.pausePanel.activeSelf == false)
            {
                UIController.Instance.pausePanel.SetActive(true);
                Time.timeScale = 0;
                Debug.Log("Game tạm dừng tại " + System.DateTime.Now);
            }
            else
            {
                UIController.Instance.pausePanel.SetActive(false);
                Time.timeScale = 1;
                Debug.Log("Game tiếp tục tại " + System.DateTime.Now);
            }
        }
        else
        {
            Debug.LogError("UIController.Instance hoặc pausePanel chưa được gán trong GameManager_Level3!");
        }
    }

    public void ReturnToMenu()
    {
        Debug.Log("Quay về Menu từ Level 3 tại " + System.DateTime.Now);
        PlayerPrefs.SetString("LastLevel", "Level 3");
        PlayerPrefs.Save();
        Time.timeScale = 1;
        if (UIController.Instance != null && UIController.Instance.pausePanel != null)
        {
            UIController.Instance.pausePanel.SetActive(false);
        }
        if (skyParent != null) skyParent.SetActive(false);
        if (cloudParent != null) cloudParent.SetActive(false);
        if (key != null) key.SetActive(false);
        SceneManager.LoadScene("Menu");
    }
}