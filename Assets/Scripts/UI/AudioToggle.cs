using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class AudioToggle : MonoBehaviour
{
    [System.Serializable]
    public class ToggleEventData
    {
        public bool isOn;
    }

    [SerializeField] private AudioType audioType;
    [System.Serializable] // Unity не сериализует дженерик-классы, поэтому нужно создавать наследника с атрибутом [Serializable]
    public class BoolUnityEvent : UnityEvent<ToggleEventData> { } 
    public BoolUnityEvent OnToggleChanged;

    const string MusicOnKey = "MusicOn";
    const string SFXOnKey = "SFXOn";
    private Toggle thisToggle;
    private string audioKey;

    private GameSettings gameSettings = new GameSettings();

    private void Awake()
    {
        thisToggle = GetComponent<Toggle>();

        if (thisToggle == null)
            Debug.LogWarning($"У {gameObject} не найден компонет Toggle");

        thisToggle.onValueChanged.AddListener(HandleToggle);
        audioKey = GetAudioKey(audioType);
    }

    private void OnEnable()
    {
        SetToggleState(audioKey);
    }

    private void HandleToggle(bool isOn)
    {
        // Пробрасываем событие наружу
        OnToggleChanged?.Invoke(new ToggleEventData { isOn = isOn });
        Debug.Log(isOn);
    }

    private string GetAudioKey(AudioType _audioType)
    {
        switch (_audioType)
        {
            case AudioType.Music:
                return MusicOnKey;
            case AudioType.SFX:
                return SFXOnKey;
            default:
                Debug.LogWarning($"Неизвестный AudioType {_audioType}", this);
                return "UnknownAudioKey";
        }
    }

    private void SetToggleState(string _key)
    {
        if (PlayerPrefs.GetInt(_key, 1) == 0)
            thisToggle.isOn = false;
        else
            thisToggle.isOn = true;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt(audioKey, thisToggle.isOn ? 1 : 0);
    }

    private void OnDestroy()
    {
        thisToggle.onValueChanged.RemoveListener(HandleToggle);
    }

    private enum AudioType
    {
        Music,
        SFX
    }
}
