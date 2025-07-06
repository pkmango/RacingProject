using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0, 1)] public float volume = 1;
        public bool loop;
    }

    public Sound[] SFX;
    public Sound[] Music;
}
