using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicGroup, sfxGroup;
    private string musicVolumeParam = "MusicVolume";
    private string sfxVolumeParam = "SFXVolume";
    [SerializeField, Min(0)] private float fadeMusicDuration = 0.8f; // Длительность затухания музыки при переходе сцен
    [SerializeField] private Toggle musicToggle, sfxToggle;
    [SerializeField] private AudioLibrary library;

    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();

    // Object pooling
    [SerializeField, Min(1)] private int sfxPoolSize = 10; // Количество объектов sfx в пуле
    private List<AudioSource> sfxSources;
    private int currentIndex = 0;

    // Music management
    private AudioSource musicSource;
    private float sfxVolumeBeforeMute;
    private float musicVolumeBeforeMute;

    private GameSettings gameSettings = new GameSettings();

    private void Awake()
    {
        InitializeDictionaries();

        // Создаем источник для музыки
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;

        // Подписка
        musicToggle.onValueChanged.AddListener(ToggleMusic);
        sfxToggle.onValueChanged.AddListener(ToggleSFX);

        InitializeSfxPool();
    }

    private void Start()
    {
        GetAudioSettings();
    }

    // Создаем словари для оптимизации доступа к звукам и 
    // возможности безопасно получать значение через TryGetValue()
    private void InitializeDictionaries()
    {
        foreach (var sound in library.SFX)
        {
            sfxClips.Add(sound.name, sound.clip);
        }

        foreach (var sound in library.Music)
        {
            musicClips.Add(sound.name, sound.clip);
        }
    }

    private void InitializeSfxPool()
    {
        sfxSources = new List<AudioSource>(sfxPoolSize);

        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject obj = new GameObject($"SFX_Source_{i}");
            obj.transform.SetParent(transform); // Перенести?

            AudioSource source = obj.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxGroup;
            source.spatialBlend = 1f; // 3D-звук
            source.playOnAwake = false;

            sfxSources.Add(source);
        }
    }

    public void PlaySFX(SoundType soundType, Vector3 position = default)
    {
        string sfxName = soundType.ToString();

        if (sfxClips.TryGetValue(sfxName, out AudioClip clip))
        {
            if (position == default)
                position = Camera.main.transform.position;

            // Берём текущий источник
            AudioSource source = sfxSources[currentIndex];

            // Настраиваем и играем
            source.transform.position = position;
            source.clip = clip;
            source.Play();

            // Переходим к следующему (с зацикливанием)
            currentIndex = (currentIndex + 1) % sfxSources.Count;
        }
        else
        {
            Debug.Log($"Имя для audio '{sfxName}' не найдено");
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

        for (float t = 0; t < fadeMusicDuration; t += Time.deltaTime)
        {
            float newVolume = Mathf.Lerp(startVolume, 0.0001f, t / fadeMusicDuration); // 0.0001f ~ -80dB
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

    public void ToggleSFX(bool isOn)
    {
        ToggleAudio(isOn, sfxGroup, sfxVolumeParam, ref sfxVolumeBeforeMute);
    }

    public void ToggleMusic(bool isOn)
    {
        ToggleAudio(isOn, musicGroup, musicVolumeParam, ref musicVolumeBeforeMute);
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

    private void GetAudioSettings()
    {
        musicToggle.isOn = gameSettings.MusicOn != 0;
        sfxToggle.isOn = gameSettings.SFXOn != 0;
    }

    private void SetAudioSettings()
    {
        gameSettings.MusicOn = musicToggle.isOn? 1 : 0;
        gameSettings.SFXOn = sfxToggle.isOn ? 1 : 0;
    }

    private void OnDestroy()
    {
        musicToggle.onValueChanged.RemoveListener(ToggleMusic);
        sfxToggle.onValueChanged.RemoveListener(ToggleSFX);
        SetAudioSettings();
    }
}
