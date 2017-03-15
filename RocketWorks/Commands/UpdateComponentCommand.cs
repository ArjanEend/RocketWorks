using RocketWorks.Entities;
using RocketWorks.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketWorks.Commands
{
    [Serializable]
    public partial class UpdateComponentCommand<T> : NetworkCommandBase<T> where T : EntityContext
    {
        private IComponent component;
        private uint hash;
        
        public UpdateComponentCommand(){}

        public UpdateComponentCommand(IComponent component, uint hash)
        {
            this.component = component;
            this.hash = hash;
        }

        public override void Execute(T target, int uid)
        {
            target.Pool.ReplaceComponent(component, hash, uid);
        }
    }
}
