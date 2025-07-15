using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManageMen : MonoBehaviour
{
    public void Play()
    {
        Debug.Log("Tải cảnh Level1 tại " + System.DateTime.Now);
        SceneManager.LoadScene("Level1");
    }

    public void Exit()
    {
        Debug.Log("Thoát game tại " + System.DateTime.Now);
        Application.Quit();
    }

    public void Menu()
    {
        Debug.Log("Tải cảnh Menu tại " + System.DateTime.Now);
        SceneManager.LoadScene("Menu");
    }

    public void NextBttn()
    {
        string lastLevel = PlayerPrefs.GetString("LastLevel", "Level1");
        Debug.Log("Tải cảnh " + lastLevel + " để tiếp tục tại " + System.DateTime.Now);
        SceneManager.LoadScene(lastLevel);
    }
}