using UnityEngine;
namespace RocketWorks.State
{
    public abstract class MonoState<T> : IState<T> where T : Object
    {

        protected T entity;
        private StateFinished<T> onFinish;
        public StateFinished<T> OnFinish
        {
            get { return onFinish; }
            set { onFinish = value; }
        }

        public virtual void RegisterState(T actor)
        {
            this.entity = actor;
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