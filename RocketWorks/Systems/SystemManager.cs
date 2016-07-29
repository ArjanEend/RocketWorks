using RocketWorks.Pooling;
using System;
using System.Collections.Generic;

namespace RocketWorks.Systems
{
    public class SystemManager : ISystemManager
    {
        private EntityPool pool;

        private List<ISystem> systems;

        public SystemManager(EntityPool pool)
        {
            this.pool = pool;
        }

        public void AddSystem(ISystem system)
        {
            system.Initialize();
        }

        public void RemoveSystem(ISystem system)
        {
            
        }

        public void UpdateSystems()
        {
            for(int i = 0; i < systems.Count; i++)
            {
                systems[i].Execute();
            }
        }
    }
}
