﻿using UnityEngine;
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

        protected Contexts contexts;

        public static T Create<T>() where T : UnitySystemBase
        {
            string className = typeof(T).Name;
            RocketLog.LogFormat("New [{0}]", null, className);
            GameObject go = new GameObject(string.Format("[{0}]", className));
            T instance = go.AddComponent<T>();
            DontDestroyOnLoad(go);
            DontDestroyOnLoad(instance);
            return instance;
        }
        public virtual void Initialize(Contexts contexts)
        {
            this.contexts = contexts;
        }
        public abstract void Execute(float deltaTime);
        public abstract void Destroy();
    }
}