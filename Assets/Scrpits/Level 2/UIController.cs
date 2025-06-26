using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    public GameObject pausePanel;

    void Awake()
    {
        // Chỉ gán Instance, không dùng DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Hủy instance trùng lặp
        }
    }

    void Start()
    {
        if (pausePanel == null)
        {
            Debug.LogError("pausePanel is not assigned in UIController!");
        }
    }

    void Update()
    {
        // Thêm logic nếu cần
    }
}
