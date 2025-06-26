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

    public GameObject seaParent;
    public GameObject skyParent;
    public GameObject cloudParent;

    public Color cursedColor = new Color(0.2f, 0.4f, 0.5f);
    public Color cleanWaterColor = new Color(0f, 0.6f, 0.8f);
    public Color dayColor = new Color(1f, 0.95f, 0.8f);

    public GameObject gameOverScreen;
    public GameObject victoryUI;

    private SpriteRenderer[] seaRenderers;
    private SpriteRenderer[] skyRenderers;
    private SpriteRenderer[] cloudRenderers;

    private bool hasChangedColor = false;
    private bool hasWaterCleaned = false;

    private bool isGameOver = false;

    void Start()
    {
        if (seaParent != null) seaRenderers = seaParent.GetComponentsInChildren<SpriteRenderer>();
        if (skyParent != null) skyRenderers = skyParent.GetComponentsInChildren<SpriteRenderer>();
        if (cloudParent != null) cloudRenderers = cloudParent.GetComponentsInChildren<SpriteRenderer>();

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void CollectItem() { collectedItems++; }
    public void KillEnemy() { enemiesKilled++; }
    public void KillBoss() { isBossKilled = true; }

    public bool IsBossKilled() { return isBossKilled; }

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
            Debug.LogWarning("GameOverScreen is not assigned in GameManager!");
        }
    }

    public void Victory()
    {
        if (victoryUI != null)
        {
            victoryUI.SetActive(true);
            Debug.Log("Victory UI activated!");
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

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            if (seaRenderers != null)
                foreach (SpriteRenderer sr in seaRenderers)
                    if (sr != null) sr.color = Color.Lerp(cursedColor, cleanWaterColor, t);
            yield return null;
        }

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

    public void Pause()
    {
        if (UIController.Instance != null && UIController.Instance.pausePanel != null)
        {
            if (UIController.Instance.pausePanel.activeSelf == false)
            {
                UIController.Instance.pausePanel.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                UIController.Instance.pausePanel.SetActive(false);
                Time.timeScale = 1;
            }
        }
        else
        {
            Debug.LogError("UIController.Instance or pausePanel is not assigned!");
        }
    }

    public void ReturnToMenu()
    {
        Debug.Log("Returning to Menu scene at " + System.DateTime.Now);
        Time.timeScale = 1;
        if (UIController.Instance != null && UIController.Instance.pausePanel != null)
        {
            UIController.Instance.pausePanel.SetActive(false);
        }
        if (seaParent != null) seaParent.SetActive(false);
        if (skyParent != null) skyParent.SetActive(false);
        if (cloudParent != null) cloudParent.SetActive(false);
        SceneManager.LoadScene("Menu");
    }
}
