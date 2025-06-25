using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Vector2 lastCheckpointPos; // Định nghĩa duy nhất

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            lastCheckpointPos = transform.position;
            Debug.Log("Checkpoint reached at: " + lastCheckpointPos + " at " + System.DateTime.Now);
        }
    }
}