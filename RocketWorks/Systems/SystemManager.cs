using RocketWorks.Pooling;
using System;
using System.Collections.Generic;

namespace RocketWorks.Systems
{
    public class SystemManager : ISystemManager
    {
        private EntityPool pool;

        private Dictionary<Type, ISystem> systemBindings = new Dictionary<Type, ISystem>();

        public void AddSystem(ISystem system)
        {
            
        }

        public void RemoveSystem(ISystem system)
        {
            
        }

        public void UpdateSystems()
        {
            
        }
    }
}
