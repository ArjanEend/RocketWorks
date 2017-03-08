using System;
using RocketWorks.Entities;
using RocketWorks.Pooling;

namespace RocketWorks.Commands
{
    [Serializable]
    public partial class CreateEntityCommand : NetworkCommandBase<EntityPool>
    {
        private Entity entity;

        public CreateEntityCommand()
        {

        }

        public CreateEntityCommand(Entity entity)
        {
            this.entity = entity;
        }

        public override void Execute(EntityPool target, uint uid)
        {
            RocketLog.Log("Create entity : " + entity.CreationIndex + " : " + uid, this);
            target.AddEntity(entity, uid);
        }
    }
}