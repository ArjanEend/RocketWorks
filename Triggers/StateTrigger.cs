using System;
using UnityEngine;

[CreateAssetMenu]
public class StateTrigger : ScriptableObject
{
    public Action<bool> OnTrigger = delegate { };

    [SerializeField]
    private bool active = false;
}

public abstract class TriggerRequirement : ScriptableObject
{
    public abstract bool IsMetInternal<T>(T subject) where T : class;
}

public abstract class TriggerRequirement<R> : TriggerRequirement where R : class
{
    public override bool IsMetInternal<T>(T subject)
    {
        return IsMet(subject as R);
    }

    public abstract bool IsMet(R subject);
}