using System;
using System.Collections.Generic;
using UnityEngine;

namespace RocketWorks.States
{
    [System.Serializable]
    public class StateTransition
    {
        public State nextState;
        public StateTrigger[] triggers;
        public bool[] logicGate;

        public bool IsSatisFied(Dictionary<StateTrigger, bool> localTriggers)
        {
            for (int i = 0; i < triggers.Length; i++)
            {
                if (localTriggers.ContainsKey(triggers[i]))
                {
                    if (localTriggers[triggers[i]] == logicGate[i])
                        continue;
                    return false;
                }
                return false;
            }
            return true;
        }
    }

    [CreateAssetMenu]
    public class State : ScriptableObject
    {
        [SerializeField]
        private StateTransition[] transitions;

        public delegate void TriggerDelegate(StateTrigger trigger, object subject = null);

        public TriggerDelegate OnTriggerRaised;
        public TriggerDelegate OnTriggerLowered;

        protected void FireTriggerRaised(StateTrigger trigger, object subject = null)
        {
            OnTriggerRaised?.Invoke(trigger, subject);
        }

        protected void FireTriggerLowered(StateTrigger trigger, object subject = null)
        {
            OnTriggerLowered?.Invoke(trigger, subject);
        }

        public State CheckTransitions(Dictionary<StateTrigger, bool> localTriggers)
        {
            for (int i = 0; i < transitions.Length; i++)
            {
                if (transitions[i].IsSatisFied(localTriggers))
                {
                    return transitions[i].nextState;
                }
            }
            return this;
        }
    }

    public abstract class State<T> : State where T : class
    {
        public abstract void OnUpdate(T subject);
        public abstract void OnFixedUpdate(T subject);

        public abstract void Enter(T subject);
        public abstract void Exit(T subject);
    }
}