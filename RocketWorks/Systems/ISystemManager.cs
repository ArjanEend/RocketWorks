using System;

namespace RocketWorks.Systems
{
    public interface ISystemManager
    {
        T AddSystem<T>(T system) where T : ISystem;
        void RemoveSystem(ISystem system);
        void UpdateSystems(float deltaTime);
    }
}
