using UnityEngine;
using System.Collections;

namespace RocketWorks.System
{
    public abstract class SystemBase
    {
        public abstract void Initialize();
        public abstract void Execute();
        public abstract void Destroy();
    }
}