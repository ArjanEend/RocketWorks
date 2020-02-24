using System;
using UnityEngine;

[CreateAssetMenu]
public class StateTrigger : ScriptableObject
{
    public Action<bool> OnTrigger = delegate { };
    public bool active = false;
}
