using System;
using UnityEngine;


[CreateAssetMenu(menuName = "RocketWorks/Events/GameEvent", fileName = "GameEvent")]
public class Signal : ScriptableObject
{
    public event Action OnEmit = delegate { };
    public event Action OnEmitOnce = delegate { };

    public void Emit()
    {
        OnEmit();
        OnEmitOnce();
        OnEmitOnce = delegate { };
    }

    public virtual void Emit(object data)
    {
        Debug.LogError($"{name} Silly... this is a non-data signal");
    }
}

public abstract class Signal<T> : Signal
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

    public override void Emit(object data)
    {
        if (data is T castData)
        {
            Emit(castData);
            return;
        }
        Debug.LogError($"{name} tried to emit {data}, which is an incompatible type");
    }
}

public abstract class Signal<T1, T2> : Signal<T1>
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
