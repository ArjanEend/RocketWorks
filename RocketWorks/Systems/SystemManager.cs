using RocketWorks.Pooling;
using System;
using System.Collections.Generic;

namespace RocketWorks.Systems
{
    public class SystemManager : ISystemManager
    {
        private EntityPool pool;

        private List<ISystem> systems;

        private Dictionary<ISystem, float> storedDelays;

        public SystemManager(EntityPool pool)
        {
            this.pool = pool;
            systems = new List<ISystem>();
            storedDelays = new Dictionary<ISystem, float>();
        }

        public void AddSystem(ISystem system)
        {
            system.Initialize(pool);
            systems.Add(system);
            storedDelays.Add(system, 0f);
        }

        public void RemoveSystem(ISystem system)
        {
            if (systems.Contains(system))
            {
                systems.Remove(system);
                storedDelays.Remove(system);
            }
        }

        public void UpdateSystems()
        {
            for (int i = 0; i < systems.Count; i++)
            {
                if(storedDelays[systems[i]] <= 0f)
                {
                    storedDelays[systems[i]] = systems[i].TickRate;
                    systems[i].Execute();
                }
                storedDelays[systems[i]] -= UnityEngine.Time.deltaTime;
            }
        }
    }
}
