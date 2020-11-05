using System;
using UnityEngine;

[CreateAssetMenu]
public class StateTrigger : ScriptableObject
{
    public Action<bool> OnTrigger = delegate { };

    [SerializeField]
    private bool active = false;
}

public abstract class TriggerRequirement<T> : ScriptableObject
{
    public abstract bool IsMet(T subject);
}