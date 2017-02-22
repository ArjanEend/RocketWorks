using UnityEngine;
using System.Collections;
using RocketWorks.Pooling;
using System;

namespace RocketWorks.Systems
{
    public abstract class UnitySystemBase : MonoBehaviour, ISystem
    {
        public float TickRate
        {
            get{ return 0f; }
        }

        public static T Initialize<T>(Contexts contexts) where T : UnitySystemBase
        {
            string className = typeof(T).Name;
            RocketLog.LogFormat("New [{0}]", null, className);
            GameObject go = new GameObject(string.Format("[{0}]", className));
            T instance = go.AddComponent<T>();
            DontDestroyOnLoad(go);
            DontDestroyOnLoad(instance);
            instance.Initialize(contexts);
            return instance;
        }
        public abstract void Initialize(Contexts contexts);
        public abstract void Execute();
        public abstract void Destroy();
    }
}