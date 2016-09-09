using UnityEngine;
using System.Collections;
using RocketWorks.Grouping;
using System.Collections.Generic;
using RocketWorks.Pooling;

namespace RocketWorks.Systems
{
    public abstract class SystemBase : ISystem
    {
        protected List<Group> groups = new List<Group>();

        public abstract void Initialize(EntityPool pool);
        public abstract void Execute();
        public abstract void Destroy();

        public void AddGroup(Group group)
        {
            groups.Add(group);
        }
    }
}