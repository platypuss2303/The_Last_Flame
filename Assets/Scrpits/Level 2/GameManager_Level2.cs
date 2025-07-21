using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_Level2 : MonoBehaviour
{
    public int totalItems = 6;
    public int totalEnemies = 2;
    private int collectedItems = 0;
    private int enemiesKilled = 0;
    private bool isBossKilled = false;
    private bool hasKeyCollected = false; // Theo dõi người chơi đã nhặt chìa khóa chưa

    public GameObject seaParent;
    public GameObject skyParent;
    public GameObject cloudParent;
    public GameObject key; // Biến cho chìa khóa (gắn script Key_Level2)

    public Color cursedColor = new Color(0.2f, 0.4f, 0.5f);
    public Color cleanWaterColor = new Color(0f, 0.6f, 0.8f);
    public Color dayColor = new Color(1f, 0.95f, 0.8f);

    public GameObject gameOverScreen;
    public GameObject victoryUI;

    private SpriteRenderer[] seaRenderers;
    private SpriteRenderer[] skyRenderers;
    private SpriteRenderer[] cloudRenderers;

    private bool hasChangedColor = false;
    private bool isGameOver = false;

    void Start()
    {
        // Khởi tạo các renderer
        if (seaParent != null) seaRenderers = seaParent.GetComponentsInChildren<SpriteRenderer>();
        else Debug.LogWarning("seaParent is not assigned in GameManager_Level2!");

        if (skyParent != null) skyRenderers = skyParent.GetComponentsInChildren<SpriteRenderer>();
        else Debug.LogWarning("skyParent is not assigned in GameManager_Level2!");

        if (cloudParent != null) cloudRenderers = cloudParent.GetComponentsInChildren<SpriteRenderer>();
        else Debug.LogWarning("cloudParent is not assigned in GameManager_Level2!");

        // Tắt chìa khóa ban đầu
        if (key != null)
        {
            key.SetActive(false);
            if (key.GetComponent<Key_Level2>() == null)
            {
                Debug.LogWarning("Key object does not have Key_Level2 script attached!");
            }
            Debug.Log("Key disabled at start at position: " + key.transform.position);
        }
        else
        {
            Debug.LogWarning("Key is not assigned in GameManager_Level2!");
        }

        // Đặt màu ban đầu và chạy hiệu ứng fade
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

        SetInitialColors();
    }

    void Update()
    {
        // Đổi màu và hiện chìa khóa khi boss bị tiêu diệt
        if (!hasChangedColor && isBossKilled)
        {
            Debug.Log("Boss killed - Changing to normal colors and showing key at " + System.DateTime.Now);
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
        Debug.Log($"Collected Items: {collectedItems}/{totalItems} at " + System.DateTime.Now);
    }

    public void KillEnemy()
    {
        enemiesKilled++;
        Debug.Log($"Enemies Killed: {enemiesKilled}/{totalEnemies} at " + System.DateTime.Now);
    }

    public void KillBoss()
    {
        isBossKilled = true;
        Debug.Log($"Boss Killed: isBossKilled={isBossKilled} at " + System.DateTime.Now);
    }

    public bool IsBossKilled()
    {
        return isBossKilled;
    }

    public void GameOver()
    {
        if (isGameOver) return;

        Debug.Log("Game Over! Player has been defeated at " + System.DateTime.Now);
        isGameOver = true;

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            Time.timeScale = 1f;
            Debug.Log("Game Over screen activated at " + System.DateTime.Now);
        }
        else
        {
            Debug.LogWarning("GameOverScreen is not assigned in GameManager_Level2!");
        }
    }

    public void Victory()
    {
        if (victoryUI != null)
        {
            victoryUI.SetActive(true);
            Time.timeScale = 0; // Dừng game khi thắng, giống Level 1
            Debug.Log("Victory UI activated at " + System.DateTime.Now);
        }
        else
        {
            Debug.LogError("victoryUI is not assigned in GameManager_Level2!");
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
            if (seaRenderers != null)
                foreach (SpriteRenderer sr in seaRenderers)
                    if (sr != null) sr.color = Color.Lerp(cursedColor, cleanWaterColor, t);
            yield return null;
        }

        if (seaRenderers != null)
            foreach (SpriteRenderer sr in seaRenderers)
                if (sr != null) sr.color = cleanWaterColor;

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

        if (skyRenderers != null)
            foreach (SpriteRenderer sr in skyRenderers)
                if (sr != null) sr.color = dayColor;
        if (cloudRenderers != null)
            foreach (SpriteRenderer cr in cloudRenderers)
                if (cr != null) cr.color = dayColor;

        Debug.Log("Changed to normal colors (cleanWaterColor for sea, dayColor for sky and clouds) at " + System.DateTime.Now);
    }

    private void ShowKey()
    {
        if (key == null)
        {
            Debug.LogWarning("Key is not assigned in GameManager_Level2! at " + System.DateTime.Now);
            return;
        }
        key.SetActive(true);
        SpriteRenderer keyRenderer = key.GetComponent<SpriteRenderer>();
        if (keyRenderer == null)
        {
            Debug.LogWarning("Key has no SpriteRenderer! at " + System.DateTime.Now);
            return;
        }
        keyRenderer.sortingLayerName = "Player";
        keyRenderer.sortingOrder = 20;
        keyRenderer.enabled = true;
        keyRenderer.color = Color.yellow;
        Debug.Log($"Key appeared - Active: {key.activeSelf}, Enabled: {keyRenderer.enabled}, Sprite: {(keyRenderer.sprite != null ? keyRenderer.sprite.name : "None")}, Position: {key.transform.position}, Color: {keyRenderer.color} at " + System.DateTime.Now);
    }

    public void CollectKey()
    {
        hasKeyCollected = true;
        Debug.Log("Key collected by player at " + System.DateTime.Now);
    }

    public void OnPlayerReachDoor()
    {
        if (hasKeyCollected)
        {
            Victory();
            Debug.Log("Player reached door with key, triggering Victory at " + System.DateTime.Now);
        }
        else
        {
            Debug.Log("Player reached door but no key collected at " + System.DateTime.Now);
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
                Debug.Log("Game paused at " + System.DateTime.Now);
            }
            else
            {
                UIController.Instance.pausePanel.SetActive(false);
                Time.timeScale = 1;
                Debug.Log("Game unpaused at " + System.DateTime.Now);
            }
        }
        else
        {
            Debug.LogError("UIController.Instance or pausePanel is not assigned in GameManager_Level2!");
        }
    }

    public void ReturnToMenu()
    {
        Debug.Log("Quay về Menu từ Level 2 tại " + System.DateTime.Now);
        PlayerPrefs.SetString("LastLevel", "Level 2");
        PlayerPrefs.Save();
        Time.timeScale = 1;
        if (UIController.Instance != null && UIController.Instance.pausePanel != null)
        {
            UIController.Instance.pausePanel.SetActive(false);
        }
        if (seaParent != null) seaParent.SetActive(false);
        if (skyParent != null) skyParent.SetActive(false);
        if (cloudParent != null) cloudParent.SetActive(false);
        if (key != null) key.SetActive(false);
        SceneManager.LoadScene("Menu");
    }
    public void LoadNextLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Level 3");
        Debug.Log("Loading Level 3 at " + System.DateTime.Now);
    }
}