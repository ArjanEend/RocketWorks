﻿using RocketWorks.Pooling;
using System;
using System.Collections.Generic;

namespace RocketWorks.Entities
{
    public delegate int ContextType(Type type);

    public class EntityContext
    {
        protected Dictionary<Type, int> components = new Dictionary<Type, int>();
        private EntityPool pool;
        public EntityPool Pool { get { return pool; } }

        public EntityContext()
        {
            components = new Dictionary<Type, int>();
            pool = new EntityPool(GetIndexOf);
        }

        public int GetIndexOf(Type type)
        {
            return components[type];
        }
    }
    public class EntityContext<T1> : EntityContext
    {
        public EntityContext(): base() { components.Add(typeof(T1), 0); }
    }
    public class EntityContext<T1, T2> : EntityContext<T1>
    {
        public EntityContext() : base() { components.Add(typeof(T2), 1); }
    }
    public class EntityContext<T1, T2, T3> : EntityContext<T1, T2>
    {
        public EntityContext() : base() { components.Add(typeof(T3), 2); }
    }
    public class EntityContext<T1, T2, T3, T4> : EntityContext<T1, T2, T3>
    {
        public EntityContext() : base() { components.Add(typeof(T4), 3); }
    }
    public class EntityContext<T1, T2, T3, T4, T5> : EntityContext<T1, T2, T3, T4>
    {
        public EntityContext() : base() { components.Add(typeof(T5), 4); }
    }
    public class EntityContext<T1, T2, T3, T4, T5, T6> : EntityContext<T1, T2, T3, T4, T5>
    {
        public EntityContext() : base() { components.Add(typeof(T6), 5); }
    }
    public class EntityContext<T1, T2, T3, T4, T5, T6, T7> : EntityContext<T1, T2, T3, T4, T5, T6>
    {
        public EntityContext() : base() { components.Add(typeof(T7), 6); }
    }
    public class EntityContext<T1, T2, T3, T4, T5, T6, T7, T8>: EntityContext<T1, T2, T3, T4, T5, T6, T7>
    {
        public EntityContext() : base() { components.Add(typeof(T8), 7); }
    }
}
