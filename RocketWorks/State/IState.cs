using System;

namespace RocketWorks.State
{
    public delegate IState<T> StateFinished<T>(IState<T> next);
    public delegate IState<T> StateFinishedType<T>(Type next);

    public interface IState
    {
        void Initialize();
        void Exit();
        void OnUpdate();
        void OnFixedUpdate();
    }

    public interface IState<T> : IState
    {
        void RegisterState(T entity);
        void DispatchFinishEvent(IState<T> next);
        void DispatchFinishEvent<R>() where R : IState<T>;
        StateFinished<T> OnFinish
        {
            get;
            set;
        }
        StateFinishedType<T> OnFinishType
        {
            get;
            set;
        }
    }
}