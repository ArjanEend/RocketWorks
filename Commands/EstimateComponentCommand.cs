﻿using RocketWorks.Entities;
using RocketWorks.Networking;
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
        protected ulong ticks;

        public EstimateComponentCommmand() { }

        public EstimateComponentCommmand(IComponent component, uint hash)
        {
            IEstimatable estimatable = (IEstimatable)component;
            if (estimatable == null)
                throw new Exception("Estimatable is not implemented in this component");
            this.component = component;
            this.hash = hash;
            this.ticks = (ulong)(ServerTimeStamp.ServerNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public override void Execute(T target, int uid)
        {
            Entity ent = target.Pool.GetEntity(hash, uid);
            IEstimatable comp = (IEstimatable)ent.GetComponent(target.Pool.GetIndexOf(component.GetType()));
            ulong ticksNow = (ulong)(ServerTimeStamp.ServerNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            float timeDiff = Mathf.Min((ticksNow - ticks) * .001f, .5f);
            lock (comp)
            {
                comp.Estimate(component, timeDiff, ent.IsLocal);
            }
        }
    }
}
