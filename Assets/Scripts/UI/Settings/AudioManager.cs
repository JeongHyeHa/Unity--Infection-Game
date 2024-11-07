using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public AudioSource[] sfxSources;

    private void Awake()
    {
        if (backgroundMusic == null)
        {
            backgroundMusic = GetComponent<AudioSource>();
            if (backgroundMusic == null)
            {
                Debug.LogError("No AudioSource component found on this object.");
                return;
            }
        }

        // 배경음악 재생
        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.volume = 1.0f;
            backgroundMusic.Play();
            Debug.Log("Background music started playing.");
        }
        else
        {
            Debug.Log("Background music AudioSource is already playing.");
        }

        // 초기 설정값 로드 및 적용
        float initialMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 100.0f);
        float initialMusicVolume = PlayerPrefs.GetFloat("BGMVolume", 100.0f);
        float initialSfxVolume = PlayerPrefs.GetFloat("SfxVolume", 100.0f);

        // 초기 값을 실제로 설정
        SetMasterVolume(initialMasterVolume);
        SetMusicVolume(initialMusicVolume);
        SetSfxVolume(initialSfxVolume);
    }

    public void SetMasterVolume(float volume)
    {
        float normalizedVolume = volume; // 0~100 범위를 0~1로 변환
        backgroundMusic.volume = normalizedVolume;

        foreach (var sfxSource in sfxSources)
        {
            sfxSource.volume = normalizedVolume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume;
        }
        else
        {
            Debug.LogError("Background music AudioSource is not assigned.");
        }
    }

    public void SetSfxVolume(float volume)
    {
        foreach (var sfxSource in sfxSources)
        {
            sfxSource.volume = volume;
        }
    }
}
