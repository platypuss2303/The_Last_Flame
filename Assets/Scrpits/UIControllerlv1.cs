using UnityEngine;

public class UIControllerlv1 : MonoBehaviour
{
    public static UIControllerlv1 Instance;
    public GameObject pausePanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (pausePanel == null)
        {
            Debug.LogError("pausePanel is not assigned in UIControllerlv1!");
        }
    }
}