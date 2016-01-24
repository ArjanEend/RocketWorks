namespace RocketWorks.System
{
    public interface ISystem
    {
        void Initialize();
        void Execute();
        void Destroy();
    }
}