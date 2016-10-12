using System;
using UnityEngine;
namespace RocketWorks.State
{
    public abstract class MonoState<T> : MonoBehaviour, IState<T> where T : UnityEngine.Object
    {
        protected T actor;
        private StateFinished<T> onFinish;
        public StateFinished<T> OnFinish
        {
            get { return onFinish; }
            set { onFinish = value; }
        }

        private StateFinishedType<T> onFinishType;
        public StateFinishedType<T> OnFinishType
        {
            get { return onFinishType; }
            set { onFinishType = value; }
        }

        public virtual void RegisterState(T actor)
        {
            Debug.Log(actor);
            this.actor = actor;
        }

        public abstract void Initialize();
        public abstract void Exit();
        public abstract void OnUpdate();
        public abstract void OnFixedUpdate();

        public IState<T> DispatchFinishEvent(IState<T> next)
        {
            if (onFinish != null)
                return onFinish(next);
            
            return this;
        }

        public R DispatchFinishEvent<R>() where R : IState<T>
        {
            Type type = typeof(R);
            if (onFinishType != null)
                return (R)onFinishType(type);

            return default(R);
        }
    }
}