using UnityEngine.Events;

// Эти классы будут содержать всю логику для событий с параметрами, но мы не будем создавать их экземпляры напрямую
public abstract class GameEvent<T0> : BaseGameEvent
{
    private event UnityAction<T0> OnRaised;

    public void Raise(T0 value)
    {
        OnRaised?.Invoke(value);
    }

    public void AddListener(UnityAction<T0> listener)
    {
        OnRaised += listener;
    }

    public void RemoveListener(UnityAction<T0> listener)
    {
        OnRaised -= listener;
    }
}

public abstract class GameEvent<T0, T1> : BaseGameEvent
{
    private event UnityAction<T0, T1> OnRaised;

    public void Raise(T0 value0, T1 value1)
    {
        OnRaised?.Invoke(value0, value1);
    }

    public void AddListener(UnityAction<T0, T1> listener)
    {
        OnRaised += listener;
    }

    public void RemoveListener(UnityAction<T0, T1> listener)
    {
        OnRaised -= listener;
    }
}

public abstract class GameEvent<T0, T1, T2> : BaseGameEvent
{
    private event UnityAction<T0, T1, T2> OnRaised;

    public void Raise(T0 value0, T1 value1, T2 value2)
    {
        OnRaised?.Invoke(value0, value1, value2);
    }

    public void AddListener(UnityAction<T0, T1, T2> listener)
    {
        OnRaised += listener;
    }

    public void RemoveListener(UnityAction<T0, T1, T2> listener)
    {
        OnRaised -= listener;
    }
}