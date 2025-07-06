using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioLibrary library;
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();
    //[SerializeField] private AudioSource musicSource;
    //[SerializeField] private AudioSource sfxSource;

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

        foreach (var sound in library.SFX)
        {
            sfxClips.Add(sound.name, sound.clip);
        }

        foreach (var sound in library.Music)
        {
            musicClips.Add(sound.name, sound.clip);
        }
    }

    public void PlaySFX(string name, Vector3 point)
    {
        if (sfxClips.TryGetValue(name, out AudioClip clip))
        {
            AudioSource.PlayClipAtPoint(clip, point);
        }
    }

    public void PlayMusic(string name, Vector3 point)
    {
        if (sfxClips.TryGetValue(name, out AudioClip clip))
        {
            AudioSource.PlayClipAtPoint(clip, point);
        }
    }

    //public void PlayMusic(AudioClip clip, bool loop = true)
    //{
    //    musicSource.clip = clip;
    //    musicSource.loop = loop;
    //    musicSource.Play();
    //}

    //public void PlaySFX(AudioClip clip, float volume = 1)
    //{
    //    sfxSource.PlayOneShot(clip, volume);
    //}
}
