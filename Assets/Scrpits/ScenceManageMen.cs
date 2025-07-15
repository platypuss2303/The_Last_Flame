using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManageMen : MonoBehaviour
{
    public void Play()
    {
        Debug.Log("Loading Level1 scene at " + System.DateTime.Now);
        SceneManager.LoadScene("Level1");
    }

    public void Exit()
    {
        Debug.Log("Exit Game at " + System.DateTime.Now);
        Application.Quit();
    }

    public void Menu()
    {
        Debug.Log("Loading Menu scene at " + System.DateTime.Now);
        SceneManager.LoadScene("Menu");
    }

    public void NextBttn()
    {
        Debug.Log("Load Next Level at " + System.DateTime.Now);
    }
}