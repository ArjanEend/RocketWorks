using System;
using RocketWorks.Triggers;

namespace RocketWorks.Entities
{
    [Serializable]
    public partial class Entity : IPoolable
    {
        public delegate void ComponentChanged(IComponent comp, Entity entity);
        public delegate void TriggerStarted(TriggerBase trigger);
        public delegate void EntityEvent(Entity ent);

        [field: NonSerialized]
        public event ComponentChanged CompositionChangeEvent;
        [field: NonSerialized]
        public event ComponentChanged CompositionSubtractEvent;
        [field: NonSerialized]
        public event TriggerStarted TriggerEvent;
        [field: NonSerialized]
        public event EntityEvent DestroyEvent;

        [field: NonSerialized]
        private ContextType context;
        public ContextType Context { set { context = value; } }

        private uint creationIndex;
        public uint CreationIndex
        {
            get { return creationIndex; }
            set { creationIndex = value; }
        }
        
        private int owner = 0;
        public int Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        private bool isLocal = false;
        public bool IsLocal
        {
            get { return isLocal; }
            set { isLocal = value; }
        }

        private bool isDirty = true;
        public bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }

        private bool enabled = true;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        
        private bool alive;
        public bool Alive
        {
            get { return alive; }
        }
        
        protected IComponent[] components;
        public IComponent[] Components
        {
            get { return components; }
        }
        public int ComponentCount
        {
            get { return components.Length; }
        }

        protected int composition = 0;
        public int Composition
        {
            get { return composition; }
        }

        public Entity() { }
        public Entity(uint creationIndex)
        {
            this.creationIndex = creationIndex;
        }
        public Entity(uint creationIndex, int totalComponents) : this(creationIndex)
        {
			components = new IComponent[totalComponents];
            composition = 0;
        }

        public void SetComponentCount(int componentAmount)
        {
            alive = true;
            components = new IComponent[componentAmount];
        }

        public T GetComponent<T>() where T : IComponent
        {
            return (T)components[context(typeof(T))];
        }

        public T GetComponent<T>(int i) where T : IComponent
        {
            return (T)components[i];
        }

        public IComponent GetComponent(int i)
        {
            return components[i];
        }

        public int GetIndex<T>() where T : IComponent
        {
            return context(typeof(T));
        }
        
        public bool HasComponent<T>()
        {
            return HasComponents(1 << context(typeof(T)));
        }
        
        public bool HasComponents(int components)
        {
            return (composition & components) == components;
        }

        public bool HasComposition(int composition)
        {
            return this.composition == composition;
        }

        public void AddComponent(IComponent component)
        {
            int index = context(component.GetType());
            components[index] = component;
            composition |= 1 << index;

            CompositionChangeEvent?.Invoke(component, this);
        }

        public T AddComponent<T>() where T : IComponent, new()
        {
            T component = new T();
            return AddComponent(component);
        }

        public T AddComponent<T>(T component) where T : IComponent
        {
            int index = context(typeof(T));
            components[index] = component;
            composition |= 1 << index;

            CompositionChangeEvent?.Invoke(component, this);
            return component;
        }

        public void RemoveComponent<T>(IComponent component) where T : IComponent
        {
            for(int i = 0; i < components.Length; i++)
            {
                if(components[i] == component)
                {
                    components[i] = null;
                    composition &= ~(1 << i);
                    CompositionSubtractEvent?.Invoke(null, this);
                    return;
                }
            }
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            int i = context(typeof(T));
            if (components[i] == null)
                return;
            IComponent comp = components[i];
            components[i] = null;
            composition &= ~(1 << i);
            CompositionSubtractEvent?.Invoke(comp, this);
        }

        public void RemoveComponent(int componentIndex)
        {
            if(components[componentIndex] != null)
            {
                components[componentIndex] = null;
                composition &= ~(1 << componentIndex);
                CompositionSubtractEvent?.Invoke(null, this);
                return;
            }
        }

        public void EmitTrigger(TriggerBase trigger)
        {
            if (TriggerEvent != null)
                TriggerEvent(trigger);
        }

        public void Reset()
        {
            alive = false;
            if (DestroyEvent != null)
                DestroyEvent(this);

            components = new IComponent[components.Length];
            composition = 0;
            TriggerEvent = null;
            DestroyEvent = null;
            CompositionChangeEvent = null;
            CompositionSubtractEvent = null;
        }

        public void ReplaceComponent(IComponent component, int index)
        {
            components[index] = component;
            composition |= 1 << index;
            CompositionChangeEvent?.Invoke(component, this);
        }
    }
}
