using UnityEngine;

public class Key_Level2 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player_Level2") && gameObject.activeSelf)
        {
            Debug.Log("Player_Level2 picked up the key at " + System.DateTime.Now);
            GameManager_Level2 gameManager = FindFirstObjectByType<GameManager_Level2>();
            if (gameManager != null)
            {
                gameManager.CollectKey();
            }
            else
            {
                Debug.LogError("GameManager_Level2 not found in scene!");
            }
            gameObject.SetActive(false); 
        }
    }
}