using RocketWorks.Entity;
using System;
using System.Collections.Generic;
using RocketWorks.Grouping;

namespace RocketWorks.Pooling
{
    public class EntityPool : ObjectPool<Entity.Entity> {

        private Dictionary<Type[], Group> typeGroups = new Dictionary<Type[], Group>();

        public void GetGroup(params Type[] types)
        {

        }
    }
}
