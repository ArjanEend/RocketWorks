
using RocketWorks.Entities;
using UnityEngine;

namespace RocketWorks.Controllers
{
    public abstract class UnityControllerBase : MonoBehaviour, IEntityController
    {
        protected Contexts contexts;
        protected Entity entity;
        public Entity Entity => entity;

        public virtual void Init(Contexts contexts)
        {
            this.contexts = contexts;
        }

        public virtual void Init(Contexts contexts, Entity entity)
        {
            this.entity = entity;
            Init(contexts);
        }

        public virtual void Destroy()
        {
        }
    }

    public abstract class UnityControllerBase<EntityType> : UnityControllerBase, IEntityController<EntityType> where EntityType : Entity
    {
        private new EntityType entity = null;
        public new EntityType Entity => entity;

        public override void Init(Contexts contexts, Entities.Entity entity)
        {
            if (entity is EntityType typedEntity)
            {
                Init(contexts, typedEntity);
            }
        }

        public virtual void Init(Contexts contexts, EntityType entity)
        {
            Init(contexts);
            this.entity = entity;
        }

        void IEntityController<EntityType>.Init(Contexts contexts, EntityType entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
