using UnityEngine;

public class OneShotAudioPlayer : MonoBehaviour
{
    public AudioClip soundClip; // ไฟล์เสียงที่ต้องการเล่น
    private AudioSource audioSource;

    private static OneShotAudioPlayer instance; // Singleton instance

    private void Awake()
    {
        // ตรวจสอบว่ามี instance อยู่แล้วหรือไม่
        if (instance != null)
        {
            Destroy(gameObject); // ทำลาย GameObject นี้หากมีอยู่แล้ว
            return;
        }

        // ตั้งค่า Singleton instance
        instance = this;
        DontDestroyOnLoad(gameObject); // ไม่ให้ถูกทำลายเมื่อเปลี่ยน Scene

        // ตั้งค่า AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // ปิดการเล่นอัตโนมัติ
    }

    public void PlaySoundOnce()
    {
        if (soundClip != null && !audioSource.isPlaying)
        {
            audioSource.clip = soundClip;
            audioSource.Play();
            StartCoroutine(DestroyAfterSound());
        }
        else
        {
            Debug.LogWarning("No sound clip assigned or audio is already playing.");
        }
    }

    private System.Collections.IEnumerator DestroyAfterSound()
    {
        // รอจนกว่าเสียงจะเล่นจบ
        yield return new WaitForSeconds(soundClip.length);

        // ทำลายตัวเองเมื่อเสียงเล่นจบ
        Destroy(gameObject);
    }
}
