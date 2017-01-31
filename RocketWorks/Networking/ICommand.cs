namespace RocketWorks.Networking
{
    public interface ICommand<T>
    {
        void Execute(T target);
    }
}
