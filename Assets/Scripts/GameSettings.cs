using UnityEngine;

public class GameSettings
{
    private const string MusicOnKey = "MusicOn";
    private const string SFXOnKey = "SFXOn";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    // Состояние переключателей (1 = вкл, 0 = выкл)
    public int MusicOn
    {
        get => PlayerPrefs.GetInt(MusicOnKey, 1); // По умолчанию включено
        set => PlayerPrefs.SetInt(MusicOnKey, value);
    }

    public int SFXOn
    {
        get => PlayerPrefs.GetInt(SFXOnKey, 1); // По умолчанию включено
        set => PlayerPrefs.SetInt(SFXOnKey, value);
    }

    // Уровни громкости (в децибелах)
    public float MusicVolume
    {
        get => PlayerPrefs.GetFloat(MusicVolumeKey, 0f); // По умолчанию 0 дБ
        set => PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    public float SFXVolume
    {
        get => PlayerPrefs.GetFloat(SFXVolumeKey, 0f); // По умолчанию 0 дБ
        set => PlayerPrefs.SetFloat(SFXVolumeKey, value);
    }

    // Метод для проверки, были ли настройки сохранены ранее
    public bool HasInitialSettings()
    {
        // Проверяем наличие одного из ключей. Если он есть, считаем, что все есть.
        return PlayerPrefs.HasKey(MusicVolumeKey);
    }

    public void Save()
    {
        PlayerPrefs.Save();
    }
}
