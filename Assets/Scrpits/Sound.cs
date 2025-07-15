using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Linq; // Thêm để sử dụng Contains

public class Sound : MonoBehaviour
{
    public AudioSource jumpAudioSource;
    public AudioSource menuBackgroundAudioSource;
    public AudioSource gameBackgroundAudioSource;
    public AudioSource attackAudioSource;
    public AudioSource coinAudioSource;
    public AudioSource bossAudioSource;

    private string menuSceneName = "Menu";
    private string[] gameScenes = { "Level1", "Level 2" }; // Danh sách các scene game

    void Start()
    {
        // Đảm bảo âm thanh nền lặp lại
        if (menuBackgroundAudioSource != null)
        {
            menuBackgroundAudioSource.loop = true;
        }
        if (gameBackgroundAudioSource != null)
        {
            gameBackgroundAudioSource.loop = true;
        }

        CheckAndPlayBackground();
    }

    void Update()
    {
        CheckAndPlayBackground();
    }

    public void PlaySound(string soundType)
    {
        switch (soundType)
        {
            case "Jump":
                jumpAudioSource.Play();
                break;
            case "MenuBackground":
                if (menuBackgroundAudioSource != null)
                {
                    menuBackgroundAudioSource.Play();
                }
                break;
            case "GameBackground":
                if (gameBackgroundAudioSource != null)
                {
                    gameBackgroundAudioSource.Play();
                }
                break;
            case "Attack":
                attackAudioSource.Play();
                break;
            case "Coin":
                coinAudioSource.Play();
                break;
            case "Boss":
                bossAudioSource.Play();
                break;
            default:
                Debug.LogWarning("Unknown sound type: " + soundType);
                break;
        }
    }

    public void StopMenuBackground()
    {
        if (menuBackgroundAudioSource != null && menuBackgroundAudioSource.isPlaying)
        {
            menuBackgroundAudioSource.Stop();
        }
    }

    public void StopGameBackground()
    {
        if (gameBackgroundAudioSource != null && gameBackgroundAudioSource.isPlaying)
        {
            gameBackgroundAudioSource.Stop();
        }
    }

    private void CheckAndPlayBackground()
    {
        if (menuBackgroundAudioSource != null || gameBackgroundAudioSource != null)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            Debug.Log("Current scene: " + currentScene);

            // Phát âm thanh menu khi ở scene menu
            if (currentScene == menuSceneName)
            {
                if (menuBackgroundAudioSource != null && !menuBackgroundAudioSource.isPlaying)
                {
                    menuBackgroundAudioSource.Play();
                    Debug.Log("Playing Music Hall in Menu scene");
                }
                if (gameBackgroundAudioSource != null && gameBackgroundAudioSource.isPlaying)
                {
                    gameBackgroundAudioSource.Stop();
                    Debug.Log("Stopping Game Background in Menu scene");
                }
            }
            // Phát âm thanh game khi ở scene Level1 hoặc Level2
            else if (gameScenes.Contains(currentScene))
            {
                if (gameBackgroundAudioSource != null && !gameBackgroundAudioSource.isPlaying)
                {
                    gameBackgroundAudioSource.Play();
                    Debug.Log("Playing Game Background in " + currentScene + " scene");
                }
                if (menuBackgroundAudioSource != null && menuBackgroundAudioSource.isPlaying)
                {
                    menuBackgroundAudioSource.Stop();
                    Debug.Log("Stopping Music Hall in " + currentScene + " scene");
                }
            }
            // Dừng cả hai nếu không phải menu hoặc game
            else
            {
                if (menuBackgroundAudioSource != null && menuBackgroundAudioSource.isPlaying)
                {
                    menuBackgroundAudioSource.Stop();
                    Debug.Log("Stopping Music Hall outside Menu/Level scenes");
                }
                if (gameBackgroundAudioSource != null && gameBackgroundAudioSource.isPlaying)
                {
                    gameBackgroundAudioSource.Stop();
                    Debug.Log("Stopping Game Background outside Menu/Level scenes");
                }
            }
        }
    }
}