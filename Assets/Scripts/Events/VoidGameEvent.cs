using UnityEngine;
using UnityEngine.Events;

// Класс для событий без параметров
[CreateAssetMenu(fileName = "VoidGameEvent", menuName = "Events/Game Event (Void)")]
public class VoidGameEvent : BaseGameEvent
{
    public UnityAction OnRaised;

    public void Raise()
    {
        OnRaised?.Invoke();
    }

    public void AddListener(UnityAction listener)
    {
        OnRaised += listener;
    }

    public void RemoveListener(UnityAction listener)
    {
        OnRaised -= listener;
    }
}