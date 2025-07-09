using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicGroup, sfxGroup;

    [SerializeField] private AudioLibrary library;
    //[SerializeField] private AudioLibrary _musicLibrary;
    //[SerializeField] private AudioLibrary _sfxLibrary;

    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();
    //[SerializeField] private AudioSource musicSource;
    //[SerializeField] private AudioSource sfxSource;

    private AudioSource musicSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

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
    }

    //public void PlaySFX(string name, Vector3 point)
    //{
    //    if (sfxClips.TryGetValue(name, out AudioClip clip))
    //    {
    //        AudioSource.PlayClipAtPoint(clip, point);
    //    }
    //}

    public void PlaySFX(string sfxName, Vector3 position = default)
    {
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

    //public void PlaySFX(string sfxName, GameObject targetObject = null)
    //{
    //    if (sfxClips.TryGetValue(sfxName, out AudioClip clip))
    //    {
    //        if (targetObject == null)
    //            targetObject = CreateTempGameObject(clip.length); // Если объект не передан, то создаем временный с временем жизни clip.length

    //        AudioSource sfxSource = targetObject.AddComponent<AudioSource>();
    //        //sfxSource.clip = clip;
    //        sfxSource.spatialBlend = 1f; // Включаем 3D-звук
    //        sfxSource.outputAudioMixerGroup = sfxGroup;
    //        sfxSource.PlayOneShot(clip);
    //    }
    //}

    public void PlayMusic(string musicName)
    {
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

    //private GameObject CreateTempGameObject(float destroyTime)
    //{
    //    GameObject tempGO = new GameObject();
    //    tempGO.transform.position = Camera.main.transform.position;
    //    Destroy(tempGO, destroyTime);
    //    return tempGO;
    //}

    //public class LoopedSFX
    //{
    //    public string soundName;
    //    public AudioSource audioSource;
    //    public GameObject targetObject;
    //    public float maxDistance;
    //}

    // Класс-обертка для зацилкенных sfx, привязанных к объекту (например звук двигателя)
    private class LoopedSFX
    {
        public string soundName;
        public AudioSource audioSource;
        public GameObject targetObject;
        public float maxDistance;
    }
}
