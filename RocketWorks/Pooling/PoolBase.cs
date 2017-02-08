using System;
using System.Collections.Generic;

namespace RocketWorks.Pooling
{
    public class PoolBase<T> {
        protected bool flexible = true;
        
        protected List<T> activeObjects;
        protected List<T> idleObjects;

        public virtual bool IsDepleted
        {
            get
            {
                if (this.flexible)
                    return false;
                return idleObjects.Count != 0;
            }
        }

        public PoolBase()
        {
            activeObjects = new List<T>();
            idleObjects = new List<T>();
        }

        public PoolBase(int amount, bool flexible)
        {
            this.flexible = flexible;
            activeObjects = new List<T>();
            idleObjects = new List<T>();
        }

        public virtual T GetObject()
        {
            if(idleObjects.Count > 0)
            {
                T value = idleObjects[0];
                idleObjects.RemoveAt(0);
                activeObjects.Add(value);
                return value;
            }
            if (flexible)
            {
                T value = CreateObject();
                activeObjects.Add(value);
                return value;
            }
            else
            {
                T value = activeObjects[0];
                activeObjects.RemoveAt(0);
                activeObjects.Add(value);
                return value;
            }
        }

        public void DeactivateObject(T item)
        {
            activeObjects.Remove(item);
            idleObjects.Add(item);
        }

        protected virtual T AddObject(T instance)
        {
            idleObjects.Add(instance);
            return instance;
        }

        protected virtual T CreateObject()
        {
            throw new Exception("Poolbase can't create objects");
            return default(T);
        }
    }

    public class PoolBaseSpawn<T> : PoolBase<T> where T : new()
    {
        public PoolBaseSpawn() : base() { }
        public PoolBaseSpawn(int amount, bool flexible) : base(amount, flexible) { }

        protected virtual T CreateObject()
        {
            T instance = default(T);
            return instance;
        }
    }
}
