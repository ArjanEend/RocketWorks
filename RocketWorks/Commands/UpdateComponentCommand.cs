using RocketWorks.Entities;
using RocketWorks.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketWorks.Commands
{
    [Serializable]
    class UpdateComponentCommand : NetworkCommandBase<EntityPool>
    {
        private IComponent component;
        private uint hash;
        
        public UpdateComponentCommand(IComponent component, uint hash)
        {
            this.component = component;
            this.hash = hash;
        }

        public override void Execute(EntityPool target, uint uid)
        {
            target.ReplaceComponent(component, hash, uid);
        }
    }
}
