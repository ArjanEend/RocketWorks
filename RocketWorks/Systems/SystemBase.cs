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

        protected float tickRate = 0f;
        public float TickRate
        {
            get { return tickRate; }
        }

        public abstract void Initialize(EntityPool pool);
        public abstract void Execute();
        public abstract void Destroy();
        public virtual void Cleanup()
        {

        }

        public Group AddGroup(Group group)
        {
            groups.Add(group);
            return group;
        }

        //protected List<T> newList()
      //  {

       // }

    }
}