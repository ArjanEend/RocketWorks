using UnityEngine;
using System.Collections;

namespace RocketWorks.State
{
    public class StateMachine<T>
    {
        private T owner;

        private IState<T> currentState;
        private IState<T> previousState;

        public System.Type currentType
        {
            get { return currentState.GetType(); }
        }

        public StateMachine(T owner)
        {
            this.owner = owner;
            currentState = null;
            previousState = null;
        }

        public void Update()
        {
            //	Debug.Log ("[StateMachine] Update: " + typeof(T));
            if (currentState != null)
                currentState.Update();
        }

        public IState<T> ChangeState(IState<T> newState)
        {
            previousState = currentState;

            if (currentState != null)
                currentState.Exit();

            currentState = newState;
            newState.OnFinish += ChangeState;
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
