using System.Collections.Generic;

namespace RocketWorks.Entities
{

    public class EntityEqualityComparer : IEqualityComparer<Entity>
    {
        public static readonly IEqualityComparer<Entity> comparer = new EntityEqualityComparer<Entity>();

        public bool Equals(Entity x, Entity y)
        {
            return x == y;
        }

        public int GetHashCode(Entity obj)
        {
            return obj.CreationIndex;
        }
    }
}