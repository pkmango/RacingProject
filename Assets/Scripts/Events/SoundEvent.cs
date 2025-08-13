using UnityEngine;

[CreateAssetMenu(fileName = "SoundEvent", menuName = "Events/Game Event (SoundType)")]
public class SoundEvent : GameEvent<SoundType>
{
    // Т.к. Unity не поддерживает сериализацию generic-классов,
    // нужен конкретный класс для каждого параметра
}
