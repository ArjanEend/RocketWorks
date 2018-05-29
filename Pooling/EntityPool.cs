using System;
using System.Collections.Generic;
using RocketWorks.Grouping;
using System.Linq;
using RocketWorks.Entities;
using Entity = RocketWorks.Entities.Entity;
using RocketWorks.Triggers;

namespace RocketWorks.Pooling
{
    public interface IEntityPool
    {
        Entity GetEntity(uint hash, int uid = -1);
        Type ObjectType {get;}
        int GetIndexOf(Type t);
        void AddEntity(Entity entity, int uid = 0, bool rewriteIndex = false);
        void ReplaceComponent(IComponent component, uint hash, int uid = -1);
        IGroup GetGroup(Type type, params Type[] types);
        IGroup GetGroup(params Type[] types);
    }
    public class EntityPool<T> : ObjectPool<T>, IInstanceProvider<T>, IEntityPool where T : Entity, new()
    {
        private Dictionary<Type, Action<TriggerBase>> triggers = new Dictionary<Type, Action<TriggerBase>>();
        
        public virtual Type ObjectType { get { return typeof(T); } }

        protected uint creationCount;
        protected int componentAmount;

        private Dictionary<int, Dictionary<uint, T>> statedObjects = new Dictionary<int, Dictionary<uint, T>>();

        private Dictionary<int, Group<T>> typeGroups = new Dictionary<int, Group<T>>();
        private List<Group<T>> groupList = new List<Group<T>>();
        private Dictionary<int, PoolBase<IComponent>> pools = new Dictionary<int, PoolBase<IComponent>>();

        protected ContextType contextCallback;

        public EntityPool(ContextType contextFunction, int amountOfComponents)
        {
            this.contextCallback = contextFunction;
            this.componentAmount = amountOfComponents;
            statedObjects.Add(-1, new Dictionary<uint, T>());
        }

        IGroup IEntityPool.GetGroup(params Type[] types)
        {
            return GetGroup(types);
        }

        public virtual Group<T> GetGroup(params Type[] types)
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
                Group<T> group = new Group<T>();
                typeGroups.Add(bitMask, group);
                group.Composition = bitMask;
                for (int i = 0; i < activeObjects.Count; i++)
                {
                    if(group.HasComponents(activeObjects[i].Composition))
                        group.AddEntity(activeObjects[i]);
                }
                return group;
            }
        }

        IGroup IEntityPool.GetGroup(Type type, params Type[] types)
        {
            return GetGroup(type, types);
        }

        public Group<T> GetGroup(Type type, params Type[] types)
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
                Group<T> group = new Group<T>();
                typeGroups.Add(bitMask, group);
                group.Composition = bitMask;
                for (int i = 0; i < activeObjects.Count; i++)
                {
                    if (group.HasComponents(activeObjects[i].Composition))
                        group.AddEntity(activeObjects[i]);
                }
                return group;
            }
        }

        protected override T CreateObject()
        {
            T entity = new T();
            entity.CreationIndex = creationCount++;
            entity.SetComponentCount(componentAmount);
            entity.CompositionChangeEvent += OnCompositionAdded;
            entity.CompositionSubtractEvent += OnCompositionSubtracted;
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
            AddEntity((T)entity, uid, rewriteIndex);
        }

        public void AddEntity(T entity, int uid = 0, bool rewriteIndex = false)
        {
            if (entity == null)
            {
                RocketLog.Log("!!!!!!!!!! ENTITY IS NULL", this);
                return;
            }
            if (!statedObjects.ContainsKey(uid))
                statedObjects.Add(uid, new Dictionary<uint, T>());

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
            entity.CompositionChangeEvent += OnCompositionAdded;
            entity.CompositionSubtractEvent += OnCompositionSubtracted;
            entity.TriggerEvent += OnTriggerAdded;
            entity.DestroyEvent += OnEntityDestroyed;

            //RocketLog.Log("Added Entity: " + entity.CreationIndex + " uid: " + uid + " rewriteIndex: " + rewriteIndex, entity);
            foreach (KeyValuePair<int, Group<T>> group in typeGroups)
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
                statedObjects.Add(uid, new Dictionary<uint, T>());
            Dictionary<uint, T> coll = statedObjects[uid];
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
                statedObjects.Add(uid, new Dictionary<uint, T>());
            Dictionary<uint, T> coll = statedObjects[uid];
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

        public T GetObject(bool stated = false, int stateHolder = -1)
        {
            T ent = base.GetObject();
            ent.Owner = stateHolder;
            if (stated)
                statedObjects[stateHolder].Add(ent.CreationIndex, ent);
            return ent;
        }

        public virtual T GetCleanObject()
        {
            T ent = new T();
            ent.CreationIndex = creationCount++;
            ent.SetComponentCount(componentAmount);
            ent.Context = contextCallback;
            return ent;
        }

        protected void OnEntityDestroyed(Entity ent)
        {
            foreach (KeyValuePair<int, Group<T>> group in typeGroups)
            {
                group.Value.RemoveEntity(ent as T);
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

        protected void OnCompositionAdded(IComponent comp, Entity entity)
        {
            foreach(KeyValuePair<int, Group<T>> group in typeGroups)
            {
                if(group.Value.HasComponents(entity.Composition))
                {
                    group.Value.AddEntity(entity as T);
                }
            }
        }

        protected void OnCompositionSubtracted(IComponent comp, Entity entity)
        {
            foreach (KeyValuePair<int, Group<T>> group in typeGroups)
            {
                if (!group.Value.HasComponents(entity.Composition))
                {
                    group.Value.RemoveEntity(entity as T);
                }
            }
        }

        public T GetTypedInstance()
        {
            return GetCleanObject();
        }

        public object GetInstance()
        {
            return GetCleanObject();
        }

        public void ResetAll()
        {
            for(int i = 0; i < activeObjects.Count; i++)
            {
                activeObjects[i].Reset();
            }
        }

        internal object GetEntity()
        {
            throw new NotImplementedException();
        }
    }
}
/* 
    public class EntityPool<T> : EntityPool where T : Entity, new()
    {
        public override Type ObjectType { get { return typeof(T); } }

        public EntityPool(ContextType contextFunction, int amountOfComponents) : base (contextFunction, amountOfComponents)
        {
        }

        public Group<T> GetGroup()
        {
               
        }

        protected override Entity CreateObject()
        {
            Entity entity = new T();
            entity.CreationIndex = creationCount++;
            entity.Owner = -1;
            entity.SetComponentCount(componentAmount);
            entity.CompositionChangeEvent += OnCompositionAdded;
            entity.CompositionSubtractEvent += OnCompositionSubtracted;
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
*/