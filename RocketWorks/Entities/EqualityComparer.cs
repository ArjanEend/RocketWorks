using System.Collections.Generic;

namespace RocketWorks.Entities
{

    public class EntityEqualityComparer : IEqualityComparer<Entity>
    {
        public static readonly IEqualityComparer<Entity> comparer = new EntityEqualityComparer();

        public bool Equals(Entity x, Entity y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode(Entity obj)
        {
            return (int)obj.CreationIndex;
        }
    }
}