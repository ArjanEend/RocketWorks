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

        public abstract void Initialize(Contexts contexts);
        public abstract void Execute(float deltaTime);
        public abstract void Destroy();
        public virtual void Cleanup()
        {

        }

        public Group AddGroup(Group group)
        {
            groups.Add(group);
            return group;
        }
    }
}