using RocketWorks.Pooling;

namespace RocketWorks.Systems
{
    public interface ISystem
    {
        void Initialize(Contexts contexts);
        void Destroy();
        float TickRate { get; }
    }

    public interface ISystemExecute
    {
        void Execute(float deltaTime);
    }

    public interface ISystemUpdateFrame
    {
        void UpdateFrame(float deltaTime);
    }
}