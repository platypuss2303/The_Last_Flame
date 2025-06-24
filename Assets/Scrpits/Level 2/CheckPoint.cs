using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Respawn respawn;
    private BoxCollider2D checkPointCollider;

    void Awake()
    {
        checkPointCollider = GetComponent<BoxCollider2D>();
        respawn = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Respawn>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player_Level2"))
        {
            respawn.respawnPoint = this.gameObject; // Cập nhật checkpoint gần nhất
            checkPointCollider.enabled = false; // Vô hiệu hóa collider sau khi kích hoạt
        }
    }
}