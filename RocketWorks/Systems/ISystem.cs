using RocketWorks.Pooling;

namespace RocketWorks.Systems
{
    public interface ISystem
    {
        void Initialize(Contexts contexts);
        void Execute();
        void Destroy();
        float TickRate { get; }
    }
}