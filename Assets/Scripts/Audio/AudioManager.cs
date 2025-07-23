using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    //public static AudioManager Instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicGroup, sfxGroup;
    private string musicVolumeParam = "MusicVolume";
    private string sfxVolumeParam = "SFXVolume";
    [SerializeField, Min(0)] private float fadeDuration = 0.8f;

    [SerializeField] private AudioLibrary library;

    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();

    private AudioSource musicSource;
    private float defaultMusicVolume;
    private float defaultSFXVolume;
    private bool sfxMuted;
    private float sfxVolumeBeforeMute;
    private bool musicMuted;
    private float musicVolumeBeforeMute;

    void Awake()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}

        // Создаем словари для оптимизации доступа к звукам и 
        // возможности безопасно получать значение через TryGetValue()
        foreach (var sound in library.SFX)
        {
            sfxClips.Add(sound.name, sound.clip);
        }

        foreach (var sound in library.Music)
        {
            musicClips.Add(sound.name, sound.clip);
        }

        // Создаем источник для музыки
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;

        musicGroup.audioMixer.GetFloat(musicVolumeParam, out defaultMusicVolume);
        sfxGroup.audioMixer.GetFloat(sfxVolumeParam, out defaultSFXVolume);
    }

    public void PlaySFX(SoundType soundType, Vector3 position = default)
    {
        string sfxName = soundType.ToString();

        if (sfxClips.TryGetValue(sfxName, out AudioClip clip))
        {
            if (position == default)
                position = Camera.main.transform.position;

            GameObject sfxGO = new GameObject();
            sfxGO.transform.position = position;
            AudioSource sfxSource = sfxGO.AddComponent<AudioSource>();
            sfxSource.spatialBlend = 1f; // Включаем 3D-звук
            sfxSource.outputAudioMixerGroup = sfxGroup;
            sfxSource.PlayOneShot(clip);
            Destroy(sfxGO, clip.length);
        }
    }

    // Создаем метод-обертку чтобы иметь возможность подписываться на событие с 1 параметром через инспектор
    public void PlaySFXWrapper(SoundType soundType)
    {
        PlaySFX(soundType); // position будет = default
    }

    public void PlayMusic(SoundType soundType)
    {
        string musicName = soundType.ToString();

        if (musicClips.TryGetValue(musicName, out AudioClip clip))
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.Log($"Имя для audio '{musicName}' не найдено");
        }
    }

    private IEnumerator FadeOutAndStopMusic()
    {
        audioMixer.GetFloat(musicVolumeParam, out float startVolume);

        // Переводим из децибел в линейное значение (0-1)
        startVolume = Mathf.Pow(10, startVolume / 20);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float newVolume = Mathf.Lerp(startVolume, 0.0001f, t / fadeDuration); // 0.0001f ~ -80dB
            audioMixer.SetFloat(musicVolumeParam, Mathf.Log10(newVolume) * 20);
            yield return null;
        }

        musicSource.Stop();
        audioMixer.SetFloat(musicVolumeParam, Mathf.Log10(startVolume) * 20);
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOutAndStopMusic());
    }

    public void ToggleSFX(AudioToggle.ToggleEventData data)
    {
        sfxMuted = !data.isOn;
        ToggleAudio(data.isOn, sfxGroup, sfxVolumeParam, ref sfxVolumeBeforeMute);
        Debug.Log($"ToggleSFX {data.isOn}");
    }

    public void ToggleMusic(bool musicEnabled)
    {
        musicMuted = !musicEnabled;
        ToggleAudio(musicEnabled, musicGroup, musicVolumeParam, ref musicVolumeBeforeMute);
        Debug.Log($"ToggleMusic {musicEnabled}");
    }

    private void ToggleAudio(bool isEnabled, AudioMixerGroup audioMixerGroup, string volumeParam, ref float audioVolumeBeforeMute)
    {
        if (isEnabled)
        {
            audioMixerGroup.audioMixer.SetFloat(volumeParam, audioVolumeBeforeMute);
        }
        else
        {
            audioMixerGroup.audioMixer.GetFloat(volumeParam, out audioVolumeBeforeMute);
            audioMixerGroup.audioMixer.SetFloat(volumeParam, -80f);
        }
    }

    // Класс-обертка для зацилкенных sfx, привязанных к объекту (например звук двигателя)
    //private class LoopedSFX
    //{
    //    public string soundName;
    //    public AudioSource audioSource;
    //    public GameObject targetObject;
    //    public float maxDistance;
    //}
}
