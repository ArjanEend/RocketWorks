using System;
using RocketWorks.Entities;
using RocketWorks.Pooling;

namespace RocketWorks.Commands
{
    [Serializable]
    public partial class CreateEntityCommand<T> : NetworkCommandBase<T> where T : EntityContext
    {
        protected Entity entity;

        public CreateEntityCommand()
        {

        }

        public CreateEntityCommand(Entity entity)
        {
            this.entity = entity;
        }

        public override void Execute(T target, int uid)
        {
            target.Pool.AddEntity(entity, uid);
        }
    }
}