using RocketWorks.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketWorks.Entities
{
    public delegate int ContextType(Type type);


    class EntityContext<T1, T2, T3, T4, T5, T6> { };
    class EntityContext<T1, T2, T3, T4, T5, T6, T7> { };

    class EntityContext<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        private Dictionary<Type, int> components = new Dictionary<Type, int>();
        private EntityPool pool;

        public EntityContext()
        {
            components = new Dictionary<Type, int>();
            components.Add(typeof(T1), 0);
            components.Add(typeof(T2), 1);
            components.Add(typeof(T3), 2);
            components.Add(typeof(T4), 3);
            components.Add(typeof(T5), 4);
            components.Add(typeof(T6), 5);
            components.Add(typeof(T7), 6);
            components.Add(typeof(T8), 7);

            pool = new EntityPool(GetIndexOf);
        }

        public int GetIndexOf(Type type)
        {
            return components[type];
        }
    }
}
