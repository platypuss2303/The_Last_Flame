using UnityEngine;

public class Key_Level3 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player_Level3") && gameObject.activeSelf)
        {
            Debug.Log("Player_Level3 picked up the key at " + System.DateTime.Now);
            GameManager_Level3 gameManager = FindFirstObjectByType<GameManager_Level3>();
            if (gameManager != null)
            {
                gameManager.CollectKey();
            }
            else
            {
                Debug.LogError("GameManager_Level3 not found in scene!");
            }
            gameObject.SetActive(false);
        }
    }
}
