using System.Collections.Generic;
using System;
using UnityEngine;

namespace RocketWorks.States
{
    public class StateMachine<T> : MonoBehaviour where T : class
    {
        private T owner;
        public T Owner => owner;

        private State<T> currentState;
        private State<T> previousState;

        public Action<State<T>> StateChanged;

        private Dictionary<Type, State<T>> stateDictionary;

        private bool stateInitialized = false;

        public System.Type currentType
        {
            get { return currentState.GetType(); }
        }

        [SerializeField] private State startState;
        [SerializeField] private State[] states;

        private void Start()
        {
            this.stateDictionary = new Dictionary<Type, State<T>>();
            this.owner = GetComponent<T>();
            currentState = null;
            previousState = null;
            for (int i = 0; i < states.Length; i++)
            {
                RegisterState(states[i] as State<T>);
            }
            ChangeState(startState as State<T>);
        }

        public void RegisterState(State<T> state)
        {
            Type stateType = state.GetType();
            stateDictionary.Add(stateType, state);
        }

        private void Update()
        {
            if (!stateInitialized && currentState != null)
            {
                stateInitialized = true;
                currentState.Enter(owner);
            }
            if (currentState != null)
                currentState.OnUpdate(owner);
        }

        private void FixedUpdate()
        {
            if (currentState != null)
                currentState.OnFixedUpdate(owner);
        }

        public R ChangeState<R>() where R : State<T>
        {
            Type type = typeof(R);
            if (stateDictionary.ContainsKey(type))
                return (R)ChangeState(stateDictionary[type]);

            return default(R);
        }

        public R GetState<R>() where R : State<T>
        {
            Type type = typeof(R);
            if (stateDictionary.ContainsKey(type))
                return (R)stateDictionary[type];

            return default(R);
        }

        public State<T> ChangeState(Type type)
        {
            if (stateDictionary.ContainsKey(type))
                return ChangeState(stateDictionary[type]);

            return null;
        }

        public State<T> ChangeState(State<T> newState)
        {
            previousState = currentState;

            if (currentState != null)
            {
                currentState.Exit(owner);
            }

            currentState = newState;
            if (newState != null)
            {
            }
            stateInitialized = false;

            if (StateChanged != null)
                StateChanged(newState);

            return newState;
        }

        public void RevertToPreviousState()
        {
            if (previousState != null)
                ChangeState(previousState);
        }
    }
}
