using UnityEngine;

[CreateAssetMenu(fileName = "SoundEvent", menuName = "Events/Game Event (SoundType, Vector3, float)")]
public class SoundEvent : GameEvent<SoundType, Vector3, float>
{
    // Т.к. Unity не поддерживает сериализацию generic-классов,
    // нужен конкретный класс для каждого параметра
}
