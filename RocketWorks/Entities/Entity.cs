using System;
using RocketWorks.Triggers;

namespace RocketWorks.Entities
{
    [Serializable]
    public class Entity : IPoolable
    {
        public delegate int ComponentChanged(IComponent comp, Entity entity);
        public delegate void TriggerStarted(TriggerBase trigger);
        public delegate void EntityEvent(Entity ent);

        [field: NonSerialized]
        public event ComponentChanged CompositionChangeEvent;
        [field: NonSerialized]
        public event TriggerStarted TriggerEvent;
        [field: NonSerialized]
        public event EntityEvent DestroyEvent;

        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
        }

        private bool alive;
        public bool Alive
        {
            get { return alive; }
        }

        private IComponent[] components;

        private int composition;
        public int Composition
        {
            get { return composition; }
        }

        public Entity() {}
        public Entity(int totalComponents)
        {
			components = new IComponent[totalComponents];
            composition = 0;
        }

        public T GetComponent<T>(int i) where T : IComponent
        {
            return (T)components[i];
        }

        public bool HasComponents(int components)
        {
            return (composition & components) == components;
        }

        public bool HasComposition(int composition)
        {
            return this.composition == composition;
        }

        public T AddComponent<T>(T component) where T : IComponent
        {
            if (CompositionChangeEvent == null)
                return default(T);
            int index = CompositionChangeEvent(component, this);
			components[index] = component;
            composition |= 1 << index;
            return component;
        }

        public void RemoveComponent<T>(IComponent component) where T : IComponent
        {
            for(int i = 0; i < components.Length; i++)
            {
                if(components[i] == component)
                {
                    components[i] = null;
                    composition ^= 1 << i;
                    return;
                }
            }
        }

        public void EmitTrigger(TriggerBase trigger)
        {
            if (TriggerEvent != null)
                TriggerEvent(trigger);
        }

        public void Reset()
        {
            if (DestroyEvent != null)
                DestroyEvent(this);

            components = new IComponent[components.Length];
            composition = 0;
            TriggerEvent = null;
            DestroyEvent = null;
        }
    }
}
