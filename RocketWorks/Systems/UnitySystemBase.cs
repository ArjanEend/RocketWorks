using UnityEngine;
using System.Collections;

namespace RocketWorks.Systems
{
    public abstract class UnitySystemBase : MonoBehaviour, ISystem
    {
        public static T Initialize<T>() where T : UnitySystemBase
        {
            string className = typeof(T).Name;
            Debug.LogFormat("New [{0}]", className);
            GameObject go = new GameObject(string.Format("[{0}]", className));
            T instance = go.AddComponent<T>();
            DontDestroyOnLoad(go);
            DontDestroyOnLoad(instance);
            instance.Initialize();
            return instance;
        }
        public abstract void Initialize();
        public abstract void Execute();
        public abstract void Destroy();
    }
}