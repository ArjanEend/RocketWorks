using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RocketWorks.Pooling;

namespace RocketWorks.Pooling
{
    public class ObjectPool<T> : PoolBase<T> where T : IPoolable, new()
    {
        private int amount;

        public ObjectPool() : base() { }
        public ObjectPool(int amount, bool flexible) : base(amount, flexible) { }

        public override bool IsDepleted
        {
            get
            {
                if(!flexible && idleObjects.Count > 0)
                {
                    for (int i = 0; i < activeObjects.Count; i++)
                    {
                        if (!activeObjects[i].Alive)
                            return false;
                    }
                }
                return base.IsDepleted;
            }
        }

        protected void GeneratePool(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                CreateObject();
            }
        }

        override public T GetObject()
        {
            if(idleObjects.Count > 0)
            {
                for (int i = 0; i < activeObjects.Count; i++)
                {
                    if (!activeObjects[i].Alive)
                        return activeObjects[i];
                }
            }
            return base.GetObject();
        }
    }
}
