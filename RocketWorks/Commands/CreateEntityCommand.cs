using System;
using RocketWorks.Entities;
using RocketWorks.Pooling;

namespace RocketWorks.Commands
{
    [Serializable]
    public class CreateEntityCommand : CommandBase<EntityPool>
    {
        private Entity entity;

        public CreateEntityCommand(Entity entity)
        {
            this.entity = entity;
        }

        public override void Execute(EntityPool target)
        {
            target.AddEntity(entity);
        }
    }
}