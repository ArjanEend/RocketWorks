using System;
using UnityEngine;


[CreateAssetMenu(menuName = "RocketWorks/Events/GameEvent", fileName = "GameEvent")]
public class GameEvent : ScriptableObject
{
    public event Action OnEmit = delegate { };
    public event Action OnEmitOnce = delegate { };

    public void Emit()
    {
        OnEmit();
        OnEmitOnce();
        OnEmitOnce = delegate { };
    }
}

public abstract class GameEvent<T> : GameEvent
{
    public event Action<T> EmitV1 = delegate { };
    public event Action<T> EmitV1Once = delegate { };

    public void Emit(T value)
    {
        Emit();
        EmitV1(value);
        EmitV1Once(value);
        EmitV1Once = delegate { };
    }
}

public abstract class GameEvent<T1, T2> : GameEvent<T1>
{
    public event Action<T1, T2> EmitV2 = delegate { };
    public event Action<T1, T2> EmitV2Once = delegate { };

    public void Emit(T1 v1, T2 v2)
    {
        Emit(v1);
        EmitV2(v1, v2);
        EmitV2Once(v1, v2);
        EmitV2Once = delegate { };
    }
}
