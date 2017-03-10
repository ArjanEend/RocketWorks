using System;
using System.Collections.Generic;
using RocketWorks.Grouping;
using System.Linq;
using RocketWorks.Entities;
using Entity = RocketWorks.Entities.Entity;
using RocketWorks.Triggers;

namespace RocketWorks.Pooling
{
    public class EntityPool : ObjectPool<Entity> {

        private Dictionary<Type, Action<TriggerBase>> triggers = new Dictionary<Type, Action<TriggerBase>>();

        private uint creationCount;

        private Dictionary<int, Dictionary<uint, Entity>> statedObjects = new Dictionary<int, Dictionary<uint, Entity>>();

        private Dictionary<int, Group> typeGroups = new Dictionary<int, Group>();
        private List<Group> groupList = new List<Group>();
        //private Dictionary<Type, int> components = new Dictionary<Type, int>();
        private Dictionary<int, PoolBase<IComponent>> pools = new Dictionary<int, PoolBase<IComponent>>();

        private ContextType contextCallback;

        public EntityPool(ContextType contextFunction)
        {
            this.contextCallback = contextFunction;
            //int componentIndices = 0;
            statedObjects.Add(-1, new Dictionary<uint, Entity>());
            //for (int i = 0; i < types.Length; i++)
            //{
                //components.Add(types[i], componentIndices);
            //    pools.Add(i, new PoolBase<IComponent>());
            //    componentIndices++;
            //}
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
                Group group = new Group(types);
                typeGroups.Add(bitMask, group);
                group.Composition = bitMask;
                return group;
            }
        }

        protected override Entity CreateObject()
        {
            Entity entity = new Entity(creationCount++, 8);
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
                    RocketLog.Log("Skipping past Entity: " + entity.CreationIndex + " uid: " + uid + " rewriteIndex: " + rewriteIndex, this);
                    return;
                }
            }

            statedObjects[uid].Add(entity.CreationIndex, entity);
            entity.CompositionChangeEvent += OnCompositionChanged;
            entity.TriggerEvent += OnTriggerAdded;
            entity.DestroyEvent += OnEntityDestroyed;

            RocketLog.Log("Added Entity: " + entity.CreationIndex + " uid: " + uid + " rewriteIndex: " + rewriteIndex, this);
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
                ent.ReplaceComponent(component, contextCallback(component.GetType()));
            } else
            {
                RocketLog.Log("Component update on non-existing entity: " + hash + ", " + uid, this);
            }
        }

        public int GetIndexOf(Type t)
        {
            return contextCallback(t);
        }

        public Entity GetObject(bool stated = false, int stateHolder = -1)
        {
            Entity ent = base.GetObject();
            if (stated)
                statedObjects[stateHolder].Add(ent.CreationIndex, ent);
            return ent;
        }

        public Entity GetCleanObject()
        {
            Entity ent = new Entity(creationCount++, 8);
            ent.Context = contextCallback;
            return ent;
        }

        private void OnEntityDestroyed(Entity ent)
        {
            foreach (KeyValuePair<int, Group> group in typeGroups)
            {
                group.Value.RemoveEntity(ent);
            }
            foreach (var kv in statedObjects)
            {
                if (kv.Value.ContainsKey(ent.CreationIndex))
                    kv.Value.Remove(ent.CreationIndex);
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

        private void OnTriggerAdded(TriggerBase trigger)
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

        private void OnCompositionChanged(IComponent comp, Entity entity)
        {
            foreach(KeyValuePair<int, Group> group in typeGroups)
            {
                if(group.Value.HasComponents(entity.Composition))
                {
                    group.Value.AddEntity(entity);
                }
            }
        }
    }
}
