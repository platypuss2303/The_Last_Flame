using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    public GameObject pausePanel;
        
    void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); 
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
