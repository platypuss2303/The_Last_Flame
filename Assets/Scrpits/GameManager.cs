using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public int totalItems = 6;
    public int totalEnemies = 2;
    private int collectedItems = 0;
    private int enemiesKilled = 0;
    private bool isBossKilled = false;
    public SpriteRenderer[] skyRenderers;
    public SpriteRenderer[] middleBgRenderers;
    public SpriteRenderer[] greenBgRenderers;
    public GameObject backgroundDay;
    public GameObject backTitle;
    public GameObject decorations;
    public GameObject key;
    public Color dayColor = new Color(1f, 0.95f, 0.8f);
    public Color nightColor = new Color(0f, 0f, 0.5f);
    public bool hasChangedColor = false;
    private bool hasKeyAppeared = false;

    public bool isGameOver = false;
    public GameObject gameOverScreen;

    void Start()
    {
        Debug.Log("GameManager Start called at " + System.DateTime.Now);

        if (backgroundDay != null) { backgroundDay.SetActive(false); Debug.Log("BackGround (Day) disabled"); }
        else Debug.LogWarning("BackGround (Day) is not assigned!");

        if (backTitle != null) { backTitle.SetActive(false); Debug.Log("BackTiles disabled"); }
        else Debug.LogWarning("BackTitle is not assigned!");

        if (decorations != null) { decorations.SetActive(false); Debug.Log("Decorations disabled"); }
        else Debug.LogWarning("Decorations is not assigned!");

        if (key != null) { key.SetActive(false); Debug.Log("Key disabled at start at position: " + key.transform.position); }
        else Debug.LogWarning("Key is not assigned!");

        GameObject doorBoss = GameObject.Find("DoorBoss");
        if (doorBoss != null)
        {
            SpriteRenderer doorRenderer = doorBoss.GetComponent<SpriteRenderer>();
            if (doorRenderer != null)
            {
                doorRenderer.enabled = true;
                doorBoss.SetActive(true);
                Debug.Log($"DoorBoss initialized - Active: {doorBoss.activeSelf}, Enabled: {doorRenderer.enabled}, Sprite: {(doorRenderer.sprite != null ? doorRenderer.sprite.name : "None")}");
            }
        }

        GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");
        foreach (GameObject trap in traps)
        {
            SpriteRenderer trapRenderer = trap.GetComponent<SpriteRenderer>();
            if (trapRenderer != null)
            {
                Debug.Log($"Initial Trap {trap.name} - Active: {trap.activeSelf}, Scale: {trapRenderer.transform.localScale}, Position: {trapRenderer.transform.position}, Sprite: {(trapRenderer.sprite != null ? trapRenderer.sprite.name : "None")}");
            }
        }

        StartCoroutine(FadeToNight());
    }

    private IEnumerator FadeToNight()
    {
        float duration = 2.0f;
        float elapsedTime = 0f;

        SpriteRenderer[] allNightRenderers = new SpriteRenderer[skyRenderers.Length + middleBgRenderers.Length + greenBgRenderers.Length];
        System.Array.Copy(skyRenderers, 0, allNightRenderers, 0, skyRenderers.Length);
        System.Array.Copy(middleBgRenderers, 0, allNightRenderers, skyRenderers.Length, middleBgRenderers.Length);
        System.Array.Copy(greenBgRenderers, 0, allNightRenderers, skyRenderers.Length + middleBgRenderers.Length, greenBgRenderers.Length);

        foreach (SpriteRenderer renderer in allNightRenderers)
        {
            if (renderer != null)
            {
                renderer.color = dayColor;
                renderer.gameObject.SetActive(true);
            }
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            foreach (SpriteRenderer renderer in allNightRenderers)
            {
                if (renderer != null)
                {
                    renderer.color = Color.Lerp(dayColor, nightColor, t);
                }
            }
            yield return null;
        }

        foreach (SpriteRenderer renderer in allNightRenderers)
        {
            if (renderer != null)
            {
                renderer.color = nightColor;
            }
        }
    }

    void Update()
    {
        // Kiểm tra điều kiện để chuyển sang ngày và hiện chìa khóa
        if (!hasChangedColor && !hasKeyAppeared && collectedItems >= totalItems && enemiesKilled >= totalEnemies && isBossKilled)
        {
            Debug.Log("All conditions met - Calling ChangeToDayMode and ShowKey at " + System.DateTime.Now);
            ChangeToDayMode();
            ShowKey();
            hasChangedColor = true;
            hasKeyAppeared = true;
        }
    }

    public void CollectItem()
    {
        collectedItems++;
        Debug.Log($"Collected Items: {collectedItems}/{totalItems} at " + System.DateTime.Now);
        Debug.Log($"Current State: collectedItems={collectedItems}, enemiesKilled={enemiesKilled}, isBossKilled={isBossKilled}, hasKeyAppeared={hasKeyAppeared}, hasChangedColor={hasChangedColor} at " + System.DateTime.Now);
    }



    public void KillEnemy()
    {
        enemiesKilled++;
        Debug.Log($"Enemies Killed: {enemiesKilled}/{totalEnemies} at " + System.DateTime.Now);
        Debug.Log($"Current State: collectedItems={collectedItems}, enemiesKilled={enemiesKilled}, isBossKilled={isBossKilled}, hasKeyAppeared={hasKeyAppeared}, hasChangedColor={hasChangedColor} at " + System.DateTime.Now);
    }

    public void KillBoss()
    {
        isBossKilled = true;
        Debug.Log($"Boss Killed: isBossKilled={isBossKilled} at " + System.DateTime.Now);
        Debug.Log($"Current State: collectedItems={collectedItems}, enemiesKilled={enemiesKilled}, isBossKilled={isBossKilled}, hasKeyAppeared={hasKeyAppeared}, hasChangedColor={hasChangedColor} at " + System.DateTime.Now);
    }

    private void ShowKey()
    {
        if (key == null)
        {
            Debug.LogWarning("Key is not assigned in ShowKey! at " + System.DateTime.Now);
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

    public void ChangeToDayMode()
    {
        Debug.Log("Attempting to change to Day Mode at " + System.DateTime.Now);

        DisableNightRenderers(skyRenderers, "Sky");
        DisableNightRenderers(middleBgRenderers, "MiddleBg");
        DisableNightRenderers(greenBgRenderers, "GreenBg");

        if (backgroundDay != null)
        {
            backgroundDay.SetActive(true);
            SpriteRenderer backgroundRenderer = backgroundDay.GetComponent<SpriteRenderer>();
            if (backgroundRenderer != null)
            {
                if (backgroundRenderer.sprite == null) Debug.LogWarning("SpriteRenderer on backgroundDay has no sprite! at " + System.DateTime.Now);
                else
                {
                    backgroundRenderer.sortingLayerName = "BackgroundDay";
                    backgroundRenderer.sortingOrder = 0;
                    Debug.Log($"Set backgroundDay to Day Mode - Active: {backgroundDay.activeSelf}, SortingLayer: {backgroundRenderer.sortingLayerName}, SortingOrder: {backgroundRenderer.sortingOrder} at " + System.DateTime.Now);
                }
            }

            SpriteRenderer[] dayRenderers = backgroundDay.GetComponentsInChildren<SpriteRenderer>();
            if (dayRenderers.Length > 0) StartCoroutine(FadeToDay(dayRenderers));
            else Debug.LogWarning("No SpriteRenderers found in backgroundDay! at " + System.DateTime.Now);
        }

        if (backTitle != null)
        {
            backTitle.SetActive(true);
            TilemapRenderer tilemapRenderer = backTitle.GetComponent<TilemapRenderer>();
            if (tilemapRenderer != null)
            {
                tilemapRenderer.sortingLayerName = "BackgroundDay";
                tilemapRenderer.sortingOrder = 4;
                Debug.Log($"Set backTitle (Tilemap) to Day Mode - Active: {backTitle.activeSelf}, SortingLayer: {tilemapRenderer.sortingLayerName}, SortingOrder: {tilemapRenderer.sortingOrder} at " + System.DateTime.Now);
            }

            SpriteRenderer titleRenderer = backTitle.GetComponent<SpriteRenderer>();
            if (titleRenderer != null)
            {
                if (titleRenderer.sprite == null) Debug.LogWarning($"SpriteRenderer on backTitle has no sprite! at " + System.DateTime.Now);
                else
                {
                    titleRenderer.sortingLayerName = "BackgroundDay";
                    titleRenderer.sortingOrder = 4;
                    Debug.Log($"Set backTitle (SpriteRenderer) to Day Mode - Active: {backTitle.activeSelf}, SortingLayer: {titleRenderer.sortingLayerName}, SortingOrder: {titleRenderer.sortingOrder} at " + System.DateTime.Now);
                }
            }
        }

        if (decorations != null)
        {
            decorations.SetActive(true);
            SpriteRenderer[] decoRenderers = decorations.GetComponentsInChildren<SpriteRenderer>();
            if (decoRenderers.Length > 0)
            {
                foreach (SpriteRenderer decoRenderer in decoRenderers)
                {
                    if (decoRenderer != null)
                    {
                        if (decoRenderer.sprite == null)
                        {
                            Debug.LogWarning($"SpriteRenderer {decoRenderer.name} in decorations has no sprite! at " + System.DateTime.Now);
                            continue;
                        }

                        decoRenderer.gameObject.SetActive(true);
                        decoRenderer.color = dayColor;
                        decoRenderer.sortingLayerName = "BackgroundDay";
                        decoRenderer.sortingOrder = 3;
                        Debug.Log($"Set {decoRenderer.name} (Decoration) to Day Mode - Active: {decoRenderer.gameObject.activeSelf}, Color: {dayColor}, SortingLayer: {decoRenderer.sortingLayerName}, SortingOrder: {decoRenderer.sortingOrder}, Position: {decoRenderer.transform.position}, Scale: {decoRenderer.transform.localScale} at " + System.DateTime.Now);
                    }
                }
            }
            else Debug.LogWarning("No SpriteRenderers found in decorations! at " + System.DateTime.Now);
        }

        // Cập nhật Sorting Layer và Order cho Traps
        GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");
        foreach (GameObject trap in traps)
        {
            SpriteRenderer trapRenderer = trap.GetComponent<SpriteRenderer>();
            if (trapRenderer != null)
            {
                trapRenderer.sortingLayerName = "Player";
                trapRenderer.sortingOrder = 20;
                trapRenderer.enabled = true;
                trap.SetActive(true);
                Debug.Log($"Trap {trap.name} - Active: {trap.activeSelf}, Enabled: {trapRenderer.enabled}, Sprite: {(trapRenderer.sprite != null ? trapRenderer.sprite.name : "None")} at " + System.DateTime.Now);
            }
        }

        // Cập nhật Sorting Layer và Order cho Doors
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            SpriteRenderer doorRenderer = door.GetComponent<SpriteRenderer>();
            if (doorRenderer != null)
            {
                doorRenderer.sortingLayerName = "Player";
                doorRenderer.sortingOrder = 20;
                doorRenderer.enabled = true;
                door.SetActive(true);
                Debug.Log($"Door {door.name} - Active: {door.activeSelf}, Enabled: {doorRenderer.enabled}, Sprite: {(doorRenderer.sprite != null ? doorRenderer.sprite.name : "None")} at " + System.DateTime.Now);
            }
        }

        // Cập nhật Sorting Layer và Order cho Enemies, SpamEnemies, và Boss
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            SpriteRenderer enemyRenderer = enemy.GetComponent<SpriteRenderer>();
            if (enemyRenderer != null)
            {
                enemyRenderer.sortingLayerName = "Player";
                enemyRenderer.sortingOrder = 15;
                enemyRenderer.enabled = true;
                enemy.SetActive(true);
                Debug.Log($"Enemy {enemy.name} - Active: {enemy.activeSelf}, Enabled: {enemyRenderer.enabled}, Sprite: {(enemyRenderer.sprite != null ? enemyRenderer.sprite.name : "None")} at " + System.DateTime.Now);
            }
        }

        GameObject[] spamEnemies = GameObject.FindGameObjectsWithTag("SpamEnemy");
        foreach (GameObject spamEnemy in spamEnemies)
        {
            SpriteRenderer spamEnemyRenderer = spamEnemy.GetComponent<SpriteRenderer>();
            if (spamEnemyRenderer != null)
            {
                spamEnemyRenderer.sortingLayerName = "Player";
                spamEnemyRenderer.sortingOrder = 15;
                spamEnemyRenderer.enabled = true;
                spamEnemy.SetActive(true);
                Debug.Log($"SpamEnemy {spamEnemy.name} - Active: {spamEnemy.activeSelf}, Enabled: {spamEnemyRenderer.enabled}, Sprite: {(spamEnemyRenderer.sprite != null ? spamEnemyRenderer.sprite.name : "None")} at " + System.DateTime.Now);
            }
        }

        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
        foreach (GameObject boss in bosses)
        {
            SpriteRenderer bossRenderer = boss.GetComponent<SpriteRenderer>();
            if (bossRenderer != null)
            {
                bossRenderer.sortingLayerName = "Player";
                bossRenderer.sortingOrder = 15;
                bossRenderer.enabled = true;
                boss.SetActive(true);
                Debug.Log($"Boss {boss.name} - Active: {boss.activeSelf}, Enabled: {bossRenderer.enabled}, Sprite: {(bossRenderer.sprite != null ? bossRenderer.sprite.name : "None")} at " + System.DateTime.Now);
            }
        }

        Debug.Log("Changed to Day Mode at " + System.DateTime.Now);
    }

    private IEnumerator FadeToDay(SpriteRenderer[] renderers)
    {
        Debug.Log("FadeToDay started with " + renderers.Length + " renderers at " + System.DateTime.Now);
        float duration = 2.0f;
        float elapsedTime = 0f;

        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.color = nightColor;
                renderer.gameObject.SetActive(true);
            }
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer != null)
                {
                    Color targetColor = renderer.name.Contains("MiddleBg") ? Color.white : dayColor;
                    renderer.color = Color.Lerp(nightColor, targetColor, t);
                }
            }
            yield return null;
        }

        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer != null)
            {
                Color targetColor = renderer.name.Contains("MiddleBg") ? Color.white : dayColor;
                renderer.color = targetColor;
                renderer.sortingLayerName = "BackgroundDay";
                renderer.sortingOrder = renderer.name.Contains("Sky") ? 0 :
                                      (renderer.name.Contains("MiddleBg") ? 1 :
                                      (renderer.name.Contains("GreenBg") ? 2 : 3));
                Debug.Log($"Set {renderer.name} to Day Mode - Active: {renderer.gameObject.activeSelf}, Color: {renderer.color}, SortingLayer: {renderer.sortingLayerName}, SortingOrder: {renderer.sortingOrder}, Position: {renderer.transform.position}, Scale: {renderer.transform.localScale} at " + System.DateTime.Now);
            }
        }
    }

    void DisableNightRenderers(SpriteRenderer[] renderers, string type)
    {
        if (renderers != null && renderers.Length > 0)
        {
            foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer != null && renderer.gameObject.activeSelf)
                {
                    renderer.gameObject.SetActive(false);
                    Debug.Log($"{renderer.name} ({type}) disabled at " + System.DateTime.Now);
                }
            }
        }
        else
        {
            Debug.LogWarning($"{type} Renderers array is empty or not assigned! at " + System.DateTime.Now);
        }
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
            Debug.LogWarning("GameOverScreen is not assigned in GameManager!");
        }
    }
}