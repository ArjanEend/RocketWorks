using RocketWorks.Entities;
using RocketWorks.Pooling;

namespace RocketWorks.Networking
{
    [System.Serializable]
    public class CreateEntityCommand : ICommand<EntityPool>
    {
        private Entity entity;

        public CreateEntityCommand(Entity entity)
        {
            this.entity = entity;
        }

        public void Execute(EntityPool target)
        {
            target.AddEntity(entity);
        }
    }
}