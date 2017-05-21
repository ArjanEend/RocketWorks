using RocketWorks.Entities;
using RocketWorks.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketWorks.Commands
{
    [Serializable]
    public partial class UpdateYourComponentCommand<T> : NetworkCommandBase<T> where T : EntityContext
    {
        protected IComponent component;
        protected uint hash;

        public UpdateYourComponentCommand() { }

        public UpdateYourComponentCommand(IComponent component, uint hash)
        {
            this.component = component;
            this.hash = hash;
        }

        public override void Execute(T target, int uid)
        {
            lock (component)
            {
                target.Pool.ReplaceComponent(component, hash, -1);
            }
        }
    }
}
