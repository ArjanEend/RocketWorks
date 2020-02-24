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

        public bool IsSatisFied(Dictionary<string, StateTrigger> localTriggers)
        {
            for (int i = 0; i < triggers.Length; i++)
            {
                if (localTriggers.ContainsKey(triggers[i].name))
                {
                    if (localTriggers[triggers[i].name].active == logicGate[i])
                        continue;
                    return false;
                }
                if (triggers[i].active == logicGate[i])
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

        public State CheckTransitions(Dictionary<string, StateTrigger> localTriggers)
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
        /*public virtual void Enter(MonoBehaviour subject)
        {

        }
        public virtual void Exit(MonoBehaviour subject)
        {

        }
        public virtual void OnUpdate(MonoBehaviour subject)
        {
        }
        public virtual void OnFixedUpdate(MonoBehaviour subject)
        {
        }*/
    }

    public abstract class State<T> : State where T : class
    {
        /*override public void OnUpdate(MonoBehaviour subject)
        {
            OnUpdate(subject as T);
        }
        override public void OnFixedUpdate(MonoBehaviour subject)
        {
            OnFixedUpdate(subject as T);
        }
        override public void Enter(MonoBehaviour subject)
        {
            Enter(subject as T);
        }
        override public void Exit(MonoBehaviour subject)
        {
            Exit(subject as T);
        }*/
        public abstract void OnUpdate(T subject);
        public abstract void OnFixedUpdate(T subject);

        public abstract void Enter(T subject);
        public abstract void Exit(T subject);
    }
}