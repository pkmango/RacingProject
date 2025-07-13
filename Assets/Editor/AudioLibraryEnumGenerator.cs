#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using System;

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
        enumBuilder.Append(NamesAppender(library.Music));
        enumBuilder.AppendLine(",");
        enumBuilder.AppendLine(NamesAppender(library.SFX));
        //footer
        enumBuilder.AppendLine("}");

        File.WriteAllText(path, enumBuilder.ToString());
        AssetDatabase.Refresh();

        Debug.Log($"Enum SoundType сгенерирован в {path}");
    }

    private string NamesAppender(AudioLibrary.Sound[] sounds)
    {
        var validNames = sounds
            .Where(sound => sound != null)
            .Select(sound => $"    {sound.name.Replace(" ", "_").Replace("-", "_").Replace(".", "_")}");

        string enumBody = string.Join($",{Environment.NewLine}", validNames);

        return enumBody;
    }
}
#endif