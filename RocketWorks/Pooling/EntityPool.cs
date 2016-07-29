using System;
using System.Collections.Generic;
using RocketWorks.Grouping;
using System.Linq;
using RocketWorks.Entities;
using Entity = RocketWorks.Entities.Entity;

namespace RocketWorks.Pooling
{
    public class EntityPool : ObjectPool<Entity> {

        private Dictionary<int, Group> typeGroups = new Dictionary<int, Group>();
        private List<Group> groupList = new List<Group>();
        private Dictionary<Type, int> components = new Dictionary<Type, int>();

        private int componentIndices = 0;

        public EntityPool()
        {
            foreach (Type mytype in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(mytype => mytype.GetInterfaces().Contains(typeof(IComponent))))
            {
                UnityEngine.Debug.Log(mytype);
                components.Add(mytype, 1 << componentIndices);
                componentIndices++;
            }
            UnityEngine.Debug.Log(componentIndices);
        }

        public Group GetGroup(params Type[] types)
        {
            int bitMask = 0;
            for(int i = 0; i < types.Length; i++)
            {
                bitMask |= components[types[i]];
            }

            if(typeGroups.ContainsKey(bitMask))
            {
                return typeGroups[bitMask];
            } else
            {
                Group group = new Group();
                typeGroups.Add(bitMask, group);
                return group;
            }
        }

        protected override Entity CreateObject()
        {
            Entity entity = new Entity(componentIndices);
            entity.CompositionChangeEvent += OnCompositionChanged;
            return entity;
        }

        private int OnCompositionChanged(IComponent comp)
        {
            return components[comp.GetType()];
        }
    }
}
