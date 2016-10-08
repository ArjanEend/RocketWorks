using System;
namespace RocketWorks.State
{
    public abstract class StateBase<T> : IState<T>
    {
        protected T actor;

        private StateFinished<T> onFinish;
        public StateFinished<T> OnFinish
        {
            get { return onFinish; }
            set { onFinish = value; }
        }

        public void RegisterState(T actor)
        {
            this.actor = actor;
        }

        public abstract void Initialize();
        public abstract void Exit();
        public abstract void Update();
        public abstract void FixedUpdate();

        public void DispatchFinishEvent(IState<T> next)
        {
            if (onFinish != null)
                onFinish(next);
        }
    }
}