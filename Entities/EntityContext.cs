using RocketWorks.Pooling;
using System;
using System.Collections.Generic;

namespace RocketWorks.Entities
{
    public delegate int ContextType(Type type);

    public abstract class EntityContext
    {
        protected Dictionary<Type, int> components = new Dictionary<Type, int>();

        protected IEntityPool pool;
        public IEntityPool Pool { get {return pool; } }

        public virtual EntityPool<T> GetPool<T>() where T : Entity, new()
        {
            throw new NotImplementedException();
            return null;
        }

        public EntityContext()
        {
            components = new Dictionary<Type, int>();
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
    public class EntityContext<T1, T2, T3, T4, T5, T6, T7, T8, T9> : EntityContext<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public EntityContext() : base() { components.Add(typeof(T9), 8); }
    }
    public class EntityContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : EntityContext<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        public EntityContext() : base() { components.Add(typeof(T10), 9); }
    }
    public class EntityContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : EntityContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        public EntityContext() : base() { components.Add(typeof(T11), 10); }
    }
    public class EntityContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : EntityContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        public EntityContext() : base() { components.Add(typeof(T12), 11); }
    }
    public class EntityContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : EntityContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        public EntityContext() : base() { components.Add(typeof(T13), 12); }
    }
    public class EntityContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : EntityContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {
        public EntityContext() : base() { components.Add(typeof(T14), 13); }
    }
}
