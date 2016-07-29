using UnityEngine;
using System.Collections;
using RocketWorks.Grouping;
using System.Collections.Generic;

namespace RocketWorks.Systems
{
    public abstract class SystemBase : ISystem
    {
        protected List<Group> groups = new List<Group>();

        public abstract void Initialize();
        public abstract void Execute();
        public abstract void Destroy();

        public void AddGroup(Group group)
        {
            groups.Add(group);
        }
    }
}