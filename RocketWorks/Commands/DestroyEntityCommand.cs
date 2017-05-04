using RocketWorks.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketWorks.Commands
{
    public partial class DestroyEntityCommand<T> : NetworkCommandBase<T> where T : EntityContext
    {
        protected EntityReference reference;

        public DestroyEntityCommand()
        {

        }

        public DestroyEntityCommand(Entity entity)
        {
            this.reference = entity;
        }

        public override void Execute(T target, int uid)
        {
            reference.Entity.Reset();
        }
    }
}
