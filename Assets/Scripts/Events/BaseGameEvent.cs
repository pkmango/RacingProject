using UnityEngine;

// Этот класс не будет создаваться как ассет, он просто база для других
public abstract class BaseGameEvent : ScriptableObject
{
    // Оставляем поле для описания в инспекторе
    [TextArea] public string description;
}
