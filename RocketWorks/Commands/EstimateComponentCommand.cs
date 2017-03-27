using RocketWorks.Entities;
using RocketWorks.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketWorks.Commands
{
    [Serializable]
    public partial class EstimateComponentCommmand<T> : NetworkCommandBase<T> where T : EntityContext
    {
        protected IComponent component;
        protected uint hash;
        protected long ticks;

        public EstimateComponentCommmand() { }

        public EstimateComponentCommmand(IComponent component, uint hash)
        {
            IEstimatable estimatable = (IEstimatable)component;
            if (estimatable == null)
                throw new Exception("Estimatable is not implemented in this component");
            this.component = component;
            this.hash = hash;
            this.ticks = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public override void Execute(T target, int uid)
        {
            IEstimatable comp = (IEstimatable)target.Pool.GetEntity(hash, uid).GetComponent(target.Pool.GetIndexOf(component.GetType()));
            long ticksNow = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            comp.Estimate(component, (ticksNow - ticks) * .01f);
        }
    }
}
