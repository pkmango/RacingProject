#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using System;
using System.Text.RegularExpressions;

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
        string path = "Assets/Scripts/Audio/SoundType.cs";
        StringBuilder enumBuilder = new StringBuilder();

        // header
        enumBuilder.AppendLine("// Автогенерируемый enum");
        enumBuilder.AppendLine("public enum SoundType");
        enumBuilder.AppendLine("{");
        //body
        var allSounds = library.Music.Concat(library.SFX).ToArray();
        enumBuilder.AppendLine(NamesAppender(allSounds));
        //footer
        enumBuilder.AppendLine("}");

        File.WriteAllText(path, enumBuilder.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();

        Debug.Log($"Enum SoundType сгенерирован в {path}");
    }

    private string NamesAppender(AudioLibrary.Sound[] sounds)
    {
        var processedNames = sounds
            .Where(s => s != null)
            .Select(s => SanitizeName(s.name))
            .GroupBy(name => name)
            .SelectMany(g => g.Count() == 1
                ? new[] { $"    {g.Key}" }
                : g.Select((name, i) => $"    {name}_{i + 1}"))
            .ToList();

        return string.Join($",{Environment.NewLine}", processedNames);
    }

    private string SanitizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Unknown";

        // Удаляем все не-ASCII символы и заменяем спецсимволы
        name = Regex.Replace(name, @"[^\p{L}\p{Nd}_]", "_")
            .Replace("__", "_")
            .Trim('_'); ;

        if (name.Any(c => c > 127))
        {
            Debug.LogWarning($"Non-ASCII имя: {name}");
        }

        return name;
    }
}
#endif