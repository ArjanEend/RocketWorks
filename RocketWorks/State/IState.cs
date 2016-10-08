namespace RocketWorks.State
{
    public delegate IState StateFinished(IState next);

    public interface IState
    {
        void Initialize();
        void Exit();
        void Update();
        void FixedUpdate();

        void DispatchFinishEvent(IState next);
    }

    public interface IState<T> : IState
    {
        void RegisterState(T entity);
    }
}