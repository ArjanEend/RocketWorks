
using RocketWorks.Entities;
using UnityEngine;

namespace RocketWorks.Controllers
{
    public abstract class UnityControllerBase : MonoBehaviour, IController
    {
        protected Contexts contexts;

        private Entity entity = null;
        public Entity Entity
        {
            get { return entity; }
        }

        public virtual void Init(Contexts contexts)
        {
            this.contexts = contexts;
        }

        public virtual void Init(Contexts contexts, Entity entity)
        {
            Init(contexts);
            this.entity = entity;
        }

        public virtual void Destroy()
        {
        }
    }
}
