using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManageMen : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Exit()
    {
                Debug.Log("Exit Game");
        Application.Quit();
    }
    public void Menu()

    {
        SceneManager.LoadScene("Menu");
    }

    public void NextBttn()
    {
        Debug.Log("Load Next Level");
    }
}
