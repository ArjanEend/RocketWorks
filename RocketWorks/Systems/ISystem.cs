namespace RocketWorks.Systems
{
    public interface ISystem
    {
        void Initialize();
        void Execute();
        void Destroy();
    }
}