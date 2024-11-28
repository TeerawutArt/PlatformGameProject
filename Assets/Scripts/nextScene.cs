using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // ต้องใช้สำหรับ Coroutine


public class SceneController : MonoBehaviour
{


    public void LoadMainMenu()
    {
       

        StartCoroutine(LoadSceneAsyncByIndex(3)); 
    }

       public void LoadLevel1()
    {
        StartCoroutine(LoadSceneAsyncByIndex(0)); 
    }
       public void LoadLevel2()
    {
        StartCoroutine(LoadSceneAsyncByIndex(1)); 
    }
    
    public void LoadLevel3()
    {
        StartCoroutine(LoadSceneAsyncByIndex(2)); 
    }


   public void exit()
    {
        #if UNITY_EDITOR
        // หยุด Play ใน Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ออกจากเกม
        Application.Quit();
#endif
    }

    // ฟังก์ชันสำหรับโหลด Scene ปัจจุบันใหม่
    public void RestartCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name; // ดึงชื่อ Scene ปัจจุบัน
        SceneManager.LoadScene(currentSceneName); // โหลด Scene ปัจจุบันใหม่
    }

    // Coroutine สำหรับโหลด Scene แบบ Async
    private IEnumerator LoadSceneAsyncByIndex(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        // รอจนกว่าการโหลด Scene จะเสร็จสมบูรณ์
        while (!asyncLoad.isDone)
        {
            Debug.Log("Loading progress: " + (asyncLoad.progress * 100) + "%");
            yield return null;
        }
    }
}
