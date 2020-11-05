using RocketWorks.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RocketWorks.Controllers
{
    public interface IEntityController
    {
        Entity Entity { get; }
        void Init(Contexts contexts);
        void Init(Contexts contexts, Entity entity);
        void Destroy();
    }

    public interface IEntityController<T> : IEntityController where T : Entity
    {
        new T Entity { get; }
        void Init(Contexts contexts, T entity);
    }
}
