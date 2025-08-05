using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicGroup, sfxGroup;
    private readonly string musicVolumeParam = "MusicVolume";
    private readonly string sfxVolumeParam = "SFXVolume";
    [SerializeField, Min(0)] private float fadeMusicDuration = 0.8f; // Длительность затухания музыки при переходе сцен
    [SerializeField] private Toggle musicToggle, sfxToggle;
    [SerializeField] private AudioLibrary library;
    [SerializeField] private GameController gameController;

    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();

    // Object pooling
    [SerializeField, Min(1)] private int sfxPoolSize = 10; // Количество объектов sfx в пуле
    private List<AudioSource> sfxSources;
    private int currentIndex = 0;

    private AudioSource musicSource;
    private float sfxVolumeBeforeMute;
    private float musicVolumeBeforeMute;

    private readonly GameSettings gameSettings = new GameSettings();
    private PlayerController _subscribedPlayer;

    private void Awake()
    {
        InitializeDictionaries();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        InitializeSfxPool();

        // Проверяем, первый ли это запуск. Если да, считываем значения по умолчанию из микшера и сохраняем их
        if (!gameSettings.HasInitialSettings())
        {
            Debug.Log("Первый запуск: сохраняем громкость по умолчанию из микшера.");
            audioMixer.GetFloat(musicVolumeParam, out float defaultMusicVol);
            audioMixer.GetFloat(sfxVolumeParam, out float defaultSfxVol);

            gameSettings.MusicVolume = defaultMusicVol;
            gameSettings.SFXVolume = defaultSfxVol;

            // Сохраняем сразу, чтобы при следующем запуске HasInitialSettings() сработало корректно
            gameSettings.Save();
        }

        if (gameController != null)
        {
            // Подписываемся на глобальное событие спауна игрока.
            // Это безопасно делать в Awake, так как мы знаем, что событие будет вызвано не раньше Start()
            gameController.OnPlayerSpawned.AddListener(HandlePlayerSpawned);
            // Регистрируем колбэк на уничтожение самого GameController
            gameController.OnControllerDestroyed += CleanUpGameControllerSubscription;
        }
        else
        {
            Debug.Log("У AudioManager отсутствует ссылка на GameController");
        }
        
    }

    private void Start()
    {
        LoadAndApplySettings();

        musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
    }

    // Создаем словари для оптимизации доступа к звукам и возможности безопасно получать значение через TryGetValue()
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
            obj.transform.SetParent(transform);

            AudioSource source = obj.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxGroup;
            source.spatialBlend = 1f; // 3D-звук
            source.playOnAwake = false;

            sfxSources.Add(source);
        }
    }

    // Метод для начальной загрузки и применения настроек
    private void LoadAndApplySettings()
    {
        // Загружаем значения из PlayerPrefs
        musicVolumeBeforeMute = gameSettings.MusicVolume;
        sfxVolumeBeforeMute = gameSettings.SFXVolume;
        bool musicIsOn = gameSettings.MusicOn != 0;
        bool sfxIsOn = gameSettings.SFXOn != 0;

        // Устанавливаем состояние UI без вызова событий
        musicToggle.SetIsOnWithoutNotify(musicIsOn);
        sfxToggle.SetIsOnWithoutNotify(sfxIsOn);

        // Применяем громкость к микшеру
        audioMixer.SetFloat(musicVolumeParam, musicIsOn ? musicVolumeBeforeMute : -80f);
        audioMixer.SetFloat(sfxVolumeParam, sfxIsOn ? sfxVolumeBeforeMute : -80f);

        Debug.Log($"Настройки Audio загружены: MusicOn={musicIsOn} (vol:{musicVolumeBeforeMute}dB), SFXOn={sfxIsOn} (vol:{sfxVolumeBeforeMute}dB)");
    }

    private void HandlePlayerSpawned(PlayerController newPlayer)
    {
        CleanUpPlayerSubscriptions(); // Если по какой-то причине мы уже подписаны на старый экземпляр
        _subscribedPlayer = newPlayer;

        _subscribedPlayer.weaponController.onPlaySFX.AddListener(PlaySFX);

        // Регистрируем наш метод очистки, который будет вызван при уничтожении игрока
        _subscribedPlayer.OnDestroyCallback += CleanUpPlayerSubscriptions;
    }

    // Отписывается от событий текущего игрока
    private void CleanUpPlayerSubscriptions()
    {
        if (_subscribedPlayer == null) return;

        _subscribedPlayer.weaponController.onPlaySFX.RemoveListener(PlaySFX);
        _subscribedPlayer.OnDestroyCallback -= CleanUpPlayerSubscriptions; // Отписываемся и от колбэка
        _subscribedPlayer = null;
    }

    // Отписываемся от событий GameController
    private void CleanUpGameControllerSubscription()
    {
        if (gameController == null) return;

        gameController.OnPlayerSpawned.RemoveListener(HandlePlayerSpawned);
        gameController.OnControllerDestroyed -= CleanUpGameControllerSubscription;
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

    public void OnMusicToggleChanged(bool isOn)
    {
        audioMixer.SetFloat(musicVolumeParam, isOn ? musicVolumeBeforeMute : -80f);
    }

    public void OnSFXToggleChanged(bool isOn)
    {
        audioMixer.SetFloat(sfxVolumeParam, isOn ? sfxVolumeBeforeMute : -80f);
    }

    // Этот метод понадобится, когда будет добавлен слайдер громкости
    public void SetMusicVolume(float volume) // volume от 0 до 1 со слайдера
    {
        // Переводим линейное значение (0-1) в логарифмическое (дБ)
        float dbVolume = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        musicVolumeBeforeMute = dbVolume; // Сохраняем новое значение "до выключения"
        gameSettings.MusicVolume = dbVolume; // Сохраняем в настройки

        // Если музыка включена, сразу применяем новую громкость
        if (musicToggle.isOn)
            audioMixer.SetFloat(musicVolumeParam, dbVolume);
    }

    private void OnDestroy()
    {
        musicToggle.onValueChanged.RemoveAllListeners();
        sfxToggle.onValueChanged.RemoveAllListeners();

        // Считываем и сохраняем финальное состояние в PlayerPrefs
        gameSettings.MusicOn = musicToggle.isOn ? 1 : 0;
        gameSettings.SFXOn = sfxToggle.isOn ? 1 : 0;
        gameSettings.Save();
        Debug.Log("Финальные настройки Audio сохранены в PlayerPrefs.");

        // Вызываем оба метода очистки на случай, если AudioManager уничтожается раньше, чем GameController или Player
        CleanUpGameControllerSubscription();
        CleanUpPlayerSubscriptions();
    }
}
