using RocketWorks.Entities;
using RocketWorks.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketWorks.Commands
{
    [Serializable]
    public partial class RemoveComponentCommand<T> : NetworkCommandBase<T> where T : EntityContext
    {
        protected int component;
        protected uint hash;

        public RemoveComponentCommand() { }

        public RemoveComponentCommand(int component, uint hash)
        {
            this.component = component;
            this.hash = hash;
        }

        public override void Execute(T target, int uid)
        {
            lock (target.Pool)
            {
                target.Pool.GetEntity(hash, uid).RemoveComponent(component);
            }
        }
    }
}
