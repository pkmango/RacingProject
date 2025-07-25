using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class AudioToggle : MonoBehaviour
{
    [Serializable]
    public class ToggleEventData
    {
        public bool isOn;
    }

    [SerializeField] private AudioType audioType;
    [Serializable] // Unity не сериализует дженерик-классы, поэтому нужно создавать наследника с атрибутом [Serializable]
    public class BoolUnityEvent : UnityEvent<ToggleEventData> { } 
    public BoolUnityEvent OnToggleChanged;

    private Toggle thisToggle;
    private enum AudioType
    {
        Music,
        SFX
    }
    private GameSettings gameSettings = new GameSettings();
    private Dictionary<AudioType, (Func<int> getter, Action<int> setter)> audioSettings;

    private void Awake()
    {
        thisToggle = GetComponent<Toggle>();
        if (thisToggle == null)
            Debug.LogWarning($"У {gameObject} не найден компонет Toggle");

        thisToggle.onValueChanged.AddListener(HandleToggle);

        audioSettings = new Dictionary<AudioType, (Func<int>, Action<int>)>
        {
            { AudioType.Music, (() => gameSettings.MusicOn, value => gameSettings.MusicOn = value) },
            { AudioType.SFX, (() => gameSettings.SFXOn, value => gameSettings.SFXOn = value) }
        };

        LoadAudioSettings();
    }

    private void HandleToggle(bool isOn)
    {
        // Пробрасываем событие наружу
        OnToggleChanged?.Invoke(new ToggleEventData { isOn = isOn });
        Debug.Log(isOn);
    }

    void LoadAudioSettings()
    {
        if (audioSettings.TryGetValue(audioType, out var settings))
        {
            thisToggle.isOn = settings.getter() != 0;
        }
        else
        {
            Debug.LogWarning($"Неизвестный AudioType {audioType}, значение не получено", this);
        }
    }

    void SaveAudioSettings()
    {
        if (audioSettings.TryGetValue(audioType, out var settings))
        {
            settings.setter(thisToggle.isOn ? 1 : 0);
        }
        else
        {
            Debug.LogWarning($"Неизвестный AudioType {audioType}, значение не сохранено", this);
        }
    }

    private void OnDestroy()
    {
        thisToggle.onValueChanged.RemoveListener(HandleToggle);
        SaveAudioSettings();
    }
}
