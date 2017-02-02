using RocketWorks.Pooling;

namespace RocketWorks.Systems
{
    public interface ISystem
    {
        void Initialize(EntityPool pool);
        void Execute();
        void Destroy();
        float TickRate { get; }
    }
}