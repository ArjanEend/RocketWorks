using RocketWorks.Pooling;
using System;
using System.Collections.Generic;

namespace RocketWorks.Systems
{
    public class SystemManager : ISystemManager
    {
        private Contexts contexts;

        private List<ISystem> systems;

        private Dictionary<ISystem, float> storedDelays;

        public SystemManager(Contexts contexts)
        {
            this.contexts = contexts;
            systems = new List<ISystem>();
            storedDelays = new Dictionary<ISystem, float>();
        }

        public T AddSystem<T>(T system) where T : ISystem
        {
            system.Initialize(contexts);
            systems.Add(system);
            storedDelays.Add(system, system.TickRate);
            return (T)system;
        }

        public void RemoveSystem(ISystem system)
        {
            if (systems.Contains(system))
            {
                systems.Remove(system);
                storedDelays.Remove(system);
            }
        }

        public void UpdateSystems(float deltaTime)
        {
            for (int i = 0; i < systems.Count; i++)
            {
                if(storedDelays[systems[i]] <= 0f)
                {
                    storedDelays[systems[i]] = systems[i].TickRate;
                    systems[i].Execute(deltaTime);
                }
                storedDelays[systems[i]] -= deltaTime;
            }
        }
    }
}
