using System;
using System.Collections.Generic;
using RocketWorks.Grouping;
using System.Linq;
using RocketWorks.Entities;
using Entity = RocketWorks.Entities.Entity;
using RocketWorks.Triggers;

namespace RocketWorks.Pooling
{
    public class EntityPool : ObjectPool<Entity>, IInstanceProvider<Entity>
    {
        private Dictionary<Type, Action<TriggerBase>> triggers = new Dictionary<Type, Action<TriggerBase>>();
        
        public virtual Type ObjectType { get { return typeof(Entity); } }

        protected uint creationCount;
        protected int componentAmount;

        private Dictionary<int, Dictionary<uint, Entity>> statedObjects = new Dictionary<int, Dictionary<uint, Entity>>();

        private Dictionary<int, Group> typeGroups = new Dictionary<int, Group>();
        private List<Group> groupList = new List<Group>();
        //private Dictionary<Type, int> components = new Dictionary<Type, int>();
        private Dictionary<int, PoolBase<IComponent>> pools = new Dictionary<int, PoolBase<IComponent>>();

        protected ContextType contextCallback;

        public EntityPool(ContextType contextFunction, int amountOfComponents)
        {
            this.contextCallback = contextFunction;
            this.componentAmount = amountOfComponents;
            statedObjects.Add(-1, new Dictionary<uint, Entity>());
        }

        public Group GetGroup(params Type[] types)
        {
            int bitMask = 0;
            for(int i = 0; i < types.Length; i++)
            {
                bitMask |= 1 << contextCallback(types[i]);
            }

            if(typeGroups.ContainsKey(bitMask))
            {
                return typeGroups[bitMask];
            } else
            {
                Group group = new Group();
                typeGroups.Add(bitMask, group);
                group.Composition = bitMask;
                return group;
            }
        }

        public Group GetGroup(Type type, params Type[] types)
        {
            int bitMask = 0;
            bitMask |= 1 << contextCallback(type);
            if(types != null)
            for (int i = 0; i < types.Length; i++)
            {
                bitMask |= 1 << contextCallback(types[i]);
            }

            if (typeGroups.ContainsKey(bitMask))
            {
                return typeGroups[bitMask];
            }
            else
            {
                Group group = new Group();
                typeGroups.Add(bitMask, group);
                group.Composition = bitMask;
                return group;
            }
        }

        protected override Entity CreateObject()
        {
            Entity entity = new Entity();
            entity.CreationIndex = creationCount++;
            entity.SetComponentCount(componentAmount);
            entity.CompositionChangeEvent += OnCompositionChanged;
            entity.Context = contextCallback;
            entity.TriggerEvent += OnTriggerAdded;
            entity.DestroyEvent += OnEntityDestroyed;
            return entity;
        }

        protected Entity CreateObject(bool stated)
        {
            Entity entity = CreateObject();
            return entity;
        }

        public void AddEntity(Entity entity, int uid = 0, bool rewriteIndex = false)
        {
            if (entity == null)
                return;
            if (!statedObjects.ContainsKey(uid))
                statedObjects.Add(uid, new Dictionary<uint, Entity>());

            if (statedObjects[uid].ContainsKey(entity.CreationIndex))
            {
                if(rewriteIndex)
                {
                    entity.CreationIndex = creationCount++;
                }
                else
                {
                    RocketLog.Log("Skipping past Entity: " + entity.CreationIndex + " uid: " + uid + " rewriteIndex: " + rewriteIndex, entity);
                    return;
                }
            }

            statedObjects[uid].Add(entity.CreationIndex, entity);
            entity.Owner = uid;
            entity.CompositionChangeEvent += OnCompositionChanged;
            entity.TriggerEvent += OnTriggerAdded;
            entity.DestroyEvent += OnEntityDestroyed;

            //RocketLog.Log("Added Entity: " + entity.CreationIndex + " uid: " + uid + " rewriteIndex: " + rewriteIndex, entity);
            foreach (KeyValuePair<int, Group> group in typeGroups)
            {
                if (group.Value.HasComponents(entity.Composition))
                {
                    group.Value.AddEntity(entity);
                }
            }
        }

        public void ReplaceComponent(IComponent component, uint hash, int uid = -1)
        {
            if (!statedObjects.ContainsKey(uid))
                statedObjects.Add(uid, new Dictionary<uint, Entity>());
            Dictionary<uint, Entity> coll = statedObjects[uid];
            if(coll.ContainsKey(hash))
            {
                Entity ent = coll[hash];
                lock (ent)
                {
                    ent.ReplaceComponent(component, contextCallback(component.GetType()));
                }
            } else
            {
                RocketLog.Log("Component update on non-existing entity: " + hash + ", " + uid + ", " + component.GetType(), component);
            }
        }

        public Entity GetEntity(uint hash, int uid = -1)
        {
            if (!statedObjects.ContainsKey(uid))
                statedObjects.Add(uid, new Dictionary<uint, Entity>());
            Dictionary<uint, Entity> coll = statedObjects[uid];
            if (coll.ContainsKey(hash))
            {
                return coll[hash];
            }
            else
            {
                RocketLog.Log("Can't find entity: " + hash + ", " + uid + ", " + componentAmount, this);
            }
            return null;
        }

        public int GetIndexOf(Type t)
        {
            return contextCallback(t);
        }

        public Entity GetObject(bool stated = false, int stateHolder = -1)
        {
            Entity ent = base.GetObject();
            ent.Owner = stateHolder;
            if (stated)
                statedObjects[stateHolder].Add(ent.CreationIndex, ent);
            return ent;
        }

        public virtual Entity GetCleanObject()
        {
            Entity ent = new Entity();
            ent.CreationIndex = creationCount++;
            ent.SetComponentCount(componentAmount);
            ent.Context = contextCallback;
            return ent;
        }

        protected void OnEntityDestroyed(Entity ent)
        {
            foreach (KeyValuePair<int, Group> group in typeGroups)
            {
                group.Value.RemoveEntity(ent);
            }

            if(statedObjects.ContainsKey(ent.Owner))
            {
                var stateHolder = statedObjects[ent.Owner];
                if (stateHolder.ContainsKey(ent.CreationIndex))
                    stateHolder.Remove(ent.CreationIndex);
            }
        }

        public void ListenTo<T>(Action<T> action) where T : TriggerBase
        {
            if(triggers.ContainsKey(typeof(T)))
            {
                triggers.Add(typeof(T), null);
            }
            triggers[typeof(T)] += action as Action<TriggerBase>;
        }

        protected void OnTriggerAdded(TriggerBase trigger)
        {
            if (triggers.ContainsKey(trigger.GetType()))
                return;

            Action<TriggerBase> triggerActions = triggers[trigger.GetType()];
            
            Delegate[] invocationList = triggerActions.GetInvocationList();
            for(int i = 0; i < invocationList.Length; i++)
            {
                if (trigger.Blocked)
                    break;

                invocationList[i].DynamicInvoke(trigger);
            }
        }

        protected void OnCompositionChanged(IComponent comp, Entity entity)
        {
            foreach(KeyValuePair<int, Group> group in typeGroups)
            {
                if(group.Value.HasComponents(entity.Composition))
                {
                    group.Value.AddEntity(entity);
                }
            }
        }

        public Entity GetTypedInstance()
        {
            return GetCleanObject();
        }

        public object GetInstance()
        {
            return GetCleanObject();
        }
    }

    public class EntityPool<T> : EntityPool where T : Entity, new()
    {
        public override Type ObjectType { get { return typeof(T); } }

        public EntityPool(ContextType contextFunction, int amountOfComponents) : base (contextFunction, amountOfComponents)
        {
        }
        
        protected override Entity CreateObject()
        {
            Entity entity = new T();
            entity.CreationIndex = creationCount++;
            entity.Owner = -1;
            entity.SetComponentCount(componentAmount);
            entity.CompositionChangeEvent += OnCompositionChanged;
            entity.Context = contextCallback;
            entity.TriggerEvent += OnTriggerAdded;
            entity.DestroyEvent += OnEntityDestroyed;
            return entity;
        }

        public override Entity GetCleanObject()
        {
            Entity ent = new T();
            ent.CreationIndex = creationCount++;
            ent.SetComponentCount(componentAmount);
            ent.Context = contextCallback;
            return ent;
        }
    }
}
