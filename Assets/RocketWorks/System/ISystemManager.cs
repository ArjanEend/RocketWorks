using System;

namespace RocketWorks.System
{
    public interface ISystemManager
    {
        void AddSystem(ISystem system);
        void RemoveSystem(ISystem system);
        void UpdateSystems();
    }
}
