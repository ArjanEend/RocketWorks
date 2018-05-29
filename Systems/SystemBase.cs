using RocketWorks.Grouping;
using System.Collections.Generic;
using RocketWorks.Pooling;

namespace RocketWorks.Systems
{
    public abstract class SystemBase : ISystem
    {
        protected List<IGroup> groups = new List<IGroup>();

        protected Contexts contexts;

        protected float tickRate = 0f;
        public float TickRate
        {
            get { return tickRate; }
        }

        public virtual void Initialize(Contexts contexts)
        {
            this.contexts = contexts;
        }
        public abstract void Execute(float deltaTime);
        public abstract void Destroy();
        public virtual void Cleanup()
        {

        }

        public IGroup AddGroup(IGroup group)
        {
            groups.Add(group);
            return group;
        }
    }
}