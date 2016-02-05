using RocketWorks.Entity;
using System;
using System.Collections.Generic;
using RocketWorks.Grouping;
using System.Linq;

namespace RocketWorks.Pooling
{
    public class EntityPool : ObjectPool<Entity.Entity> {

        private Dictionary<int, Group> typeGroups = new Dictionary<int, Group>();

        public EntityPool()
        {
            int components = 0;
            foreach (Type mytype in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(mytype => mytype.GetInterfaces().Contains(typeof(IComponent))))
            {
                components++;
            }
            UnityEngine.Debug.Log(components);
        }

        public void GetGroup(params int[] types)
        {

        }

        protected override Entity.Entity CreateObject()
        {
            return new Entity.Entity();
        }
    }
}
