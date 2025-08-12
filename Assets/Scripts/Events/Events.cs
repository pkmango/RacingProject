using UnityEngine;
using UnityEngine.Events;

// Класс для событий без параметров
[CreateAssetMenu(menuName = "Events/Game Event (Void)")]
public class GameEvent : BaseGameEvent
{
    public UnityAction OnRaised;
    public void Raise() { OnRaised?.Invoke(); }
}


// Т.к. Unity не поддерживает сериализацию generic-классов,
// здесь создаем конкретные классы для событий с параметрами
[CreateAssetMenu(menuName = "Events/Game Event (PlayerController)")]
public class PlayerControllerEvent : GameEvent<PlayerController> { }

[CreateAssetMenu(menuName = "Events/Game Event (int)")]
public class IntEvent : GameEvent<int> { }

[CreateAssetMenu(menuName = "Events/Game Event (string)")]
public class StringEvent : GameEvent<string> { }

// Можно добавлять любой необходимый тип
