using UnityEngine;
using System.Collections.Generic;
public enum SoundType
{
    MOVE,
    DEATH,
    GETITEM,
    PLACEBOMB,
    EXPLOSION,
    MUSIC,
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private SoundClipLibrary soundLibrary;

    [Header("Volume")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.5f;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Tạo 2 AudioSource riêng
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        // Load volume từ DataManager
        // musicVolume = DataManager.GetMusicVolume();
        // sfxVolume = DataManager.GetSFXVolume();
    }

    public static void PlaySound(SoundType type)
    {
        AudioClip clip = Instance.soundLibrary.GetClip(type);
        if (clip == null) return;

        if (type == SoundType.MUSIC)
        {
            Instance.musicSource.clip = clip;
            Instance.musicSource.volume = Instance.musicVolume;
            Instance.musicSource.Play();
        }
        else
        {
            Instance.sfxSource.PlayOneShot(clip, Instance.sfxVolume);
        }
    }

    public static void SetMusicVolume(float value)
    {
        Instance.musicVolume = value;
        Instance.musicSource.volume = value;
        // DataManager.SetMusicVolume(value); // Lưu volume vào DataManager
    }

    public static void SetSFXVolume(float value)
    {
        Instance.sfxVolume = value;
        Instance.sfxSource.volume = value;
        // DataManager.SetSFXVolume(value); // Lưu volume vào DataManager
    }
}