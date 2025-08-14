using UnityEngine;

[CreateAssetMenu(fileName = "SoundPositionEvent", menuName = "Events/Game Event (SoundType, Vector3)")]
public class SoundPositionEvent : GameEvent<SoundType, Vector3>
{
    // Т.к. Unity не поддерживает сериализацию generic-классов,
    // нужен конкретный класс для каждого параметра
}