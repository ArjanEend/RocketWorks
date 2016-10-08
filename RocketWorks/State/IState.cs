namespace RocketWorks.State
{
    public delegate IState<T> StateFinished<T>(IState<T> next);

    public interface IState
    {
        void Initialize();
        void Exit();
        void Update();
        void FixedUpdate();
    }

    public interface IState<T> : IState
    {
        void RegisterState(T entity);
        void DispatchFinishEvent(IState<T> next);
        StateFinished<T> OnFinish
        {
            get;
            set;
        }
    }
}