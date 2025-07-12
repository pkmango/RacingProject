#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

[CustomEditor(typeof(AudioLibrary))]
public class AudioLibraryEnumGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AudioLibrary library = (AudioLibrary)target;

        if (GUILayout.Button("Generate SoundType Enum"))
        {
            GenerateEnumFile(library);
        }
    }

    private void GenerateEnumFile(AudioLibrary library)
    {
        StringBuilder enumBuilder = new StringBuilder();
        enumBuilder.AppendLine("// Автогенерируемый enum");
        enumBuilder.AppendLine("public enum SoundType");
        enumBuilder.AppendLine("{");

        NamesAppender(library.Music, enumBuilder);
        NamesAppender(library.SFX, enumBuilder);

        enumBuilder.AppendLine("}");

        string path = "Assets/Scripts/Audio/SoundType.cs";
        File.WriteAllText(path, enumBuilder.ToString());
        AssetDatabase.Refresh();

        Debug.Log($"Enum SoundType сгенерирован в {path}");
    }

    private void NamesAppender(AudioLibrary.Sound[] sounds, StringBuilder enumBuilder)
    {
        foreach (var sound in sounds)
        {
            if (sound != null)
            {
                // Заменяем пробелы и спецсимволы
                string enumName = sound.name.Replace(" ", "_")
                                         .Replace("-", "_")
                                         .Replace(".", "_");
                enumBuilder.AppendLine($"    {enumName},");
            }
            else
            {
                Debug.LogWarning("Обнаружен пустой слот в AudioLibrary!");
            }
        }
    }
}
#endif