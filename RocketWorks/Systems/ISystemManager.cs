using System;

namespace RocketWorks.Systems
{
    public interface ISystemManager
    {
        void AddSystem(ISystem system);
        void RemoveSystem(ISystem system);
        void UpdateSystems();
    }
}
