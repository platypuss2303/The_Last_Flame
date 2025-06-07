using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    public float speed = 1f;
    private Vector3 startPos;
    private float totalLength;

    void Start()
    {
        startPos = transform.position;
        // Tính tổng chiều dài của tất cả sprite con
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length > 0)
        {
            totalLength = spriteRenderers.Length * spriteRenderers[0].bounds.size.x;
        }
    }

    void Update()
    {
        if (totalLength > 0)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);

            if (transform.position.x <= startPos.x - totalLength)
            {
                transform.position = startPos;
            }
        }
    }

    void OnDisable()
    {
        transform.position = startPos;
    }
}