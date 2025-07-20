using UnityEngine;

public class BatLv3ChaseController : MonoBehaviour
{
    public Batlv3[] batArray; // Mảng các đối tượng BatLv3

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player_Level3"))
        {
            // Kiểm tra nếu batArray không null và có phần tử
            if (batArray != null && batArray.Length > 0)
            {
                foreach (Batlv3 bat in batArray)
                {
                    if (bat != null) // Kiểm tra từng BatLv3 không null
                    {
                        bat.chase = true; // Kích hoạt rượt đuổi
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player_Level3"))
        {
            // Kiểm tra nếu batArray không null và có phần tử
            if (batArray != null && batArray.Length > 0)
            {
                foreach (Batlv3 bat in batArray)
                {
                    if (bat != null) // Kiểm tra từng BatLv3 không null
                    {
                        bat.chase = false; // Tắt rượt đuổi
                    }
                }
            }
        }
    }
}