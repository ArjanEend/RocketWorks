using RocketWorks.Pooling;

namespace RocketWorks.Systems
{
    public interface ISystem
    {
        void Initialize(Contexts contexts);
        void Execute(float deltaTime);
        void Destroy();
        float TickRate { get; }
    }
}