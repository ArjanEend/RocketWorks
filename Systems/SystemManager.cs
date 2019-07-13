using RocketWorks.Pooling;
using System;
using System.Collections.Generic;

namespace RocketWorks.Systems
{
    public class SystemManager : ISystemManager
    {
        private Contexts contexts;

        private List<ISystem> systems;

        private Dictionary<ISystem, float> storedExecutionDelays;
        private Dictionary<ISystem, float> storedUpdateDelays;

        public SystemManager(Contexts contexts)
        {
            this.contexts = contexts;
            systems = new List<ISystem>();
            storedExecutionDelays = new Dictionary<ISystem, float>();
            storedUpdateDelays = new Dictionary<ISystem, float>();
        }

        public T AddSystem<T>(T system) where T : ISystem
        {
            system.Initialize(contexts);
            systems.Add(system);
            storedExecutionDelays.Add(system, system.TickRate);
            storedUpdateDelays.Add(system, system.TickRate);
            return (T)system;
        }

        public void RemoveSystem(ISystem system)
        {
            if (systems.Contains(system))
            {
                systems.Remove(system);
                storedExecutionDelays.Remove(system);
                storedUpdateDelays.Remove(system);
            }
        }

        public void UpdateSystems(float deltaTime)
        {
            for (int i = 0; i < systems.Count; i++)
            {
                if(storedExecutionDelays[systems[i]] <= 0f)
                {
                    storedExecutionDelays[systems[i]] = systems[i].TickRate;
                    if(systems[i] is ISystemExecute executor)
                        executor.Execute(deltaTime);
                }
                storedExecutionDelays[systems[i]] -= deltaTime;
            }
        }

        public void UpdateSystemsFrame(float deltaTime)
        {
            for (int i = 0; i < systems.Count; i++)
            {
                if(storedUpdateDelays[systems[i]] <= 0f)
                {
                    storedUpdateDelays[systems[i]] = systems[i].TickRate;
                    if(systems[i] is ISystemUpdateFrame frame)
                        frame.Update(deltaTime);
                }
                storedUpdateDelays[systems[i]] -= deltaTime;
            }
        }
    }
}
