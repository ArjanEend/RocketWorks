using UnityEngine;
using System.Collections;
using RocketWorks.Pooling;
using System;

namespace RocketWorks.Systems
{
    public abstract class UnitySystemBase : ScriptableObject, ISystem
    {
        public float TickRate
        {
            get{ return 0f; }
        }

        protected Contexts contexts;

        public static void Create<T>(SystemManager manager) where T : UnitySystemBase
        {
            string className = typeof(T).Name;
            RocketLog.LogFormat("New [{0}]", null, className);
            var loadOp = UnityEngine.AddressableAssets.Addressables.LoadAsset<T>(className);
            loadOp.Completed += x => 
            {
                manager.AddSystem(x.Result);
            };
        }
        public virtual void Initialize(Contexts contexts)
        {
            this.contexts = contexts;
        }
        public abstract void Execute(float deltaTime);
        public abstract void Destroy();
    }
}