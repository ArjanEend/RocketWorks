using System;
using RocketWorks.Entities;
using RocketWorks.Pooling;

namespace RocketWorks.Commands
{
    [Serializable]
    public class CreateEntityCommand : NetworkCommandBase<EntityPool>
    {
        private Entity entity;

        public CreateEntityCommand(Entity entity)
        {
            this.entity = entity;
        }

        public override void Execute(EntityPool target, uint uid)
        {
            target.AddEntity(entity, uid);
        }
    }
}