using UnityEngine;

public class Respawn : MonoBehaviour
{
    public GameObject Player_Level2;
    public GameObject respawnPoint; // Điểm respawn, sẽ được cập nhật từ CheckPoint

    void Update()
    {
        // Phát hiện khi nhân vật rơi (chết)
        if (Player_Level2.transform.position.y < -10f) // Ngưỡng tùy chỉnh
        {
            RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        if (respawnPoint != null)
        {
            Player_Level2.transform.position = respawnPoint.transform.position;
            // Thêm offset để tránh kẹt trong collider
            Vector3 pos = Player_Level2.transform.position;
            pos.y += 1f; // Điều chỉnh độ cao để đứng trên nền tảng
            Player_Level2.transform.position = pos;
        }
        else
        {
            // Respawn tại vị trí mặc định nếu chưa có checkpoint
            Player_Level2.transform.position = new Vector3(0, 0, 0); // Tùy chỉnh vị trí
        }
    }
}