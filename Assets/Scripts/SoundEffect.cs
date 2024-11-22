using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioClip collectSound; // เก็บของ
    public AudioClip walk1; // เดิน
    public AudioClip jump; // โดด
    public AudioClip jumpOnGround; // โดดตก

    public AudioClip bgMusic; // เพลงพื้นหลัง
    public float soundVolumeBg = 0.3f;
    private AudioSource soundEffectSource;
    private AudioSource walkLoopSource;
    private AudioSource bgSoundSource;
    public static SoundEffect ShareInstance;

    void Awake()
    {
        ShareInstance = this;


    }
    void Start()
    {        // แยก AudioSource สำหรับเล่นเสียงที่เล่นครั้งเดียวและเสียงลูป
        soundEffectSource = gameObject.AddComponent<AudioSource>();
        walkLoopSource = gameObject.AddComponent<AudioSource>();
        bgSoundSource = gameObject.AddComponent<AudioSource>();
        PlayBgMusic();

    }
    public void PlayBgMusic()
    {
        bgSoundSource.clip = bgMusic;
        bgSoundSource.loop = true;
        bgSoundSource.volume = soundVolumeBg;
        bgSoundSource.Play();

    }

    public void PlaySoundEffect(string _soundEffect)
    {
        switch (_soundEffect)
        {
            case "collectSound":
                soundEffectSource.PlayOneShot(collectSound);
                break;
            case "walk":
                if (!walkLoopSource.isPlaying) // เช็คว่ากำลังเล่นเสียงเดินอยู่หรือไม่
                {
                    walkLoopSource.clip = walk1;
                    walkLoopSource.loop = true;
                    walkLoopSource.Play();
                }
                break;
            case "jump":
                soundEffectSource.PlayOneShot(jump);
                break;
            case "jumpOnGround":
                soundEffectSource.PlayOneShot(jumpOnGround);
                break;
            default:
                Debug.LogWarning("Sound effect not found: " + _soundEffect);
                break;
        }
    }

    public void StopSoundEffect(string _soundEffect)
    {
        if (_soundEffect == "walk" && walkLoopSource.isPlaying)
        {
            walkLoopSource.Stop();
        }
    }
}
