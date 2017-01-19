﻿using System;
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
        private Dictionary<int, Group> typeGroups = new Dictionary<int, Group>();
        private List<Group> groupList = new List<Group>();
        private Dictionary<Type, int> components = new Dictionary<Type, int>();

        private int componentIndices = 0;

        public EntityPool()
        {
            foreach (Type mytype in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(mytype => mytype.GetInterfaces().Contains(typeof(IComponent))))
            {
                UnityEngine.Debug.Log(mytype + " : " + (1 << componentIndices));
                components.Add(mytype, componentIndices);
                componentIndices++;
            }
            UnityEngine.Debug.Log(componentIndices);
        }

        public Group GetGroup(params Type[] types)
        {
            int bitMask = 0;
            for(int i = 0; i < types.Length; i++)
            {
                bitMask |= 1 << components[types[i]];
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

        public int GetIndexOf(Type type)
        {
            return components[type];
        }

        protected override Entity CreateObject()
        {
            Entity entity = new Entity(componentIndices);
            entity.CompositionChangeEvent += OnCompositionChanged;
            entity.TriggerEvent += OnTriggerAdded;
            entity.DestroyEvent += OnEntityDestroyed;
            return entity;
        }

        public override Entity GetObject()
        {
            Entity ent = base.GetObject();
            return ent;
        }

        private void OnEntityDestroyed(Entity ent)
        {
            foreach (KeyValuePair<int, Group> group in typeGroups)
            {
                group.Value.RemoveEntity(ent);
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

        private int OnCompositionChanged(IComponent comp, Entity entity)
        {
            foreach(KeyValuePair<int, Group> group in typeGroups)
            {
                if(group.Value.HasComponents(1 << components[comp.GetType()]))
                {
                    group.Value.AddEntity(entity);
                }
            }
            return components[comp.GetType()];
        }
    }
}