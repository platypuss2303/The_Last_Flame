using UnityEngine;

public class BatChaseController : MonoBehaviour
{
    public Bat[] batArray; // Mảng các đối tượng Bat

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player_Level2"))
        {
            // Kiểm tra nếu batArray không null và có phần tử
            if (batArray != null && batArray.Length > 0)
            {
                foreach (Bat bat in batArray)
                {
                    if (bat != null) // Kiểm tra từng Bat không null
                    {
                        bat.chase = true; // Kích hoạt rượt đuổi
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player_Level2"))
        {
            // Kiểm tra nếu batArray không null và có phần tử
            if (batArray != null && batArray.Length > 0)
            {
                foreach (Bat bat in batArray)
                {
                    if (bat != null) // Kiểm tra từng Bat không null
                    {
                        bat.chase = false; // Tắt rượt đuổi
                    }
                }
            }
        }
    }
}