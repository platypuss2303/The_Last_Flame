using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private Image transitionImage;
    public float fadeDuration = 1f;
    public string nextSceneName = "BossScene"; // Tạm thời không dùng
    public AudioClip transitionSound;
    private AudioSource audioSource;

    void Start()
    {
        transitionImage = GetComponent<Image>();
        Color color = transitionImage.color;
        color.a = 0f;
        transitionImage.color = color; // Ban đầu trong suốt

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void FadeToScene()
    {
        audioSource.PlayOneShot(transitionSound);
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = transitionImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            transitionImage.color = color;
            yield return null;
        }

        // Tạm thời comment dòng load Scene để tránh lỗi
        // SceneManager.LoadScene(nextSceneName);
        Debug.Log("Fade completed, Scene load disabled for testing.");
    }
}
