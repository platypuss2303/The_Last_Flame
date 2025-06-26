using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePause : MonoBehaviour
{


    public void Menu()
    {
        Debug.Log("Menu");
        SceneManager.LoadScene("Menu");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
