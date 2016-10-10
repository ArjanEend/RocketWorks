using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RocketWorks.State
{
    public class StateMachine<T>
    {
        private T owner;

        private IState<T> currentState;
        private IState<T> previousState;

		private Dictionary<Type, IState<T>> stateDictionary;

        public System.Type currentType
        {
            get { return currentState.GetType(); }
        }

        public StateMachine(T owner)
        {
			this.stateDictionary = new Dictionary<Type, IState<T>>();
            this.owner = owner;
            currentState = null;
            previousState = null;
        }

		public void RegisterState(IState<T> state)
		{
			Type stateType = state.GetType();
			stateDictionary.Add(stateType, state);
		}

        public void Update()
        {
            if (currentState != null)
                currentState.OnUpdate();
        }

		public void FixedUpdate()
		{
			if(currentState != null)
				currentState.OnFixedUpdate();
		}

		public IState<T> ChangeState<R>() where R : IState<T>
		{
			Type type = typeof(R);
			if(stateDictionary.ContainsKey(type))
				return ChangeState(stateDictionary[type]);

			return null;
		}

        public R GetState<R>() where R : IState<T>
        {
            Type type = typeof(R);
			if(stateDictionary.ContainsKey(type))
				return (R)stateDictionary[type];

			return default(R);
        }

		public IState<T> ChangeState(Type type)
		{
			if(stateDictionary.ContainsKey(type))
				return ChangeState(stateDictionary[type]);

			return null;
		}

        public IState<T> ChangeState(IState<T> newState)
        {
            previousState = currentState;

            if (currentState != null)
            {
                currentState.OnFinish -= ChangeState;
                currentState.OnFinishType -= ChangeState;
                currentState.Exit();
            }

            currentState = newState;
            newState.OnFinish += ChangeState;
			newState.OnFinishType += ChangeState;
            newState.RegisterState(owner);

            if (currentState != null)
                currentState.Initialize();

            return newState;
        }

        public void RevertToPreviousState()
        {
            if (previousState != null)
                ChangeState(previousState);
        }
    }
}
