using RocketWorks.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RocketWorks.Controllers
{
    public interface IController
    {
        Entity Entity { get; }
        void Init(Contexts contexts);
        void Init(Contexts contexts, Entity entity);
        void Destroy();
    }
}
