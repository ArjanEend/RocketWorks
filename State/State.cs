using System;
using System.Collections.Generic;
using UnityEngine;

namespace RocketWorks.States
{
    [System.Serializable]
    public class StateTransition
    {
        [Serializable]
        public class TriggerData
        {
            [SerializeField] public StateTrigger trigger;
            [SerializeField] public bool logicGate;
        }
        [Serializable]
        public class RequirementData
        {
            [SerializeField] public TriggerRequirement trigger;
            [SerializeField] public bool logicGate;
        }
        public State nextState;
        public TriggerData[] triggers;
        public RequirementData[] requirements;

        public bool IsSatisFied(Dictionary<StateTrigger, bool> localTriggers, object subject)
        {
            for (int i = 0; i < triggers.Length; i++)
            {
                if (localTriggers.ContainsKey(triggers[i].trigger))
                {
                    if (localTriggers[triggers[i].trigger] == triggers[i].logicGate)
                        continue;
                    return false;
                }
                return false;
            }
            for (int i = 0; i < requirements.Length; i++)
            {
                if (requirements[i].trigger.IsMetInternal(subject) == requirements[i].logicGate)
                    continue;
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

        public event TriggerDelegate OnTriggerRaised;
        public event TriggerDelegate OnTriggerLowered;

        protected void FireTriggerRaised(StateTrigger trigger, object subject = null)
        {
            OnTriggerRaised?.Invoke(trigger, subject);
        }

        protected void FireTriggerLowered(StateTrigger trigger, object subject = null)
        {
            OnTriggerLowered?.Invoke(trigger, subject);
        }

        public virtual State CheckTransitions(Dictionary<StateTrigger, bool> localTriggers, object subject)
        {
            for (int i = 0; i < transitions.Length; i++)
            {
                if (transitions[i].IsSatisFied(localTriggers, subject))
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