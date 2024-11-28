using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LogoSceneManager : MonoBehaviour
{
    public float fadeDuration = 2f; // ระยะเวลา Fade
    public float logoDisplayTime = 2f; // เวลาทั้งหมด
    public CanvasGroup canvasGroup;

    void Start()
    {
        StartCoroutine(PlayLogoScene());
    }

    IEnumerator PlayLogoScene()
    {
        // Fade In
        for (float t = 0; t <= fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1;

        // รอแสดงโลโก้
        yield return new WaitForSeconds(logoDisplayTime);

        // Fade Out
        for (float t = 0; t <= fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = 1 - (t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;

        // เปลี่ยน Scene
        SceneManager.LoadScene(3);
    }
}
