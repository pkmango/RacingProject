using UnityEngine;

public class GameSettings
{
    const string MusicOnKey = "MusicOn";
    const string SFXOnKey = "SFXOn";

    public int SFXOn
    {
        get => PlayerPrefs.GetInt(SFXOnKey, 1);
        set => PlayerPrefs.SetInt(SFXOnKey, value);
    }

    public int MusicOn
    {
        get => PlayerPrefs.GetInt(MusicOnKey, 1);
        set => PlayerPrefs.SetInt(MusicOnKey, value);
    }
}
