using RocketWorks.Networking;
using RocketWorks.Pooling;
using RocketWorks.Serialization;
using System;

namespace RocketWorks.Entities
{
    public partial struct EntityReference : IRocketable
    {
        public uint creationIndex;
        public int owner;

        public IEntityPool pool;

        public Type contextType;

        private Entity entity;
        public Entity Entity
        {
            get {
                if (entity != null || pool == null)
                    return entity;
                return entity =  pool.GetEntity(creationIndex, owner);
            }
        }

        public EntityReference(uint creationIndex, int owner)
        {
            this.creationIndex = creationIndex;
            this.owner = owner;
            pool = null;
            entity = null;
            contextType = null;
        }

        public EntityReference(Entity ent)
        {
            this.creationIndex = ent.CreationIndex;
            this.owner = ent.Owner;
            pool = null;
            entity = ent;
            contextType = ent.GetType();
        }

        public EntityReference(uint creationIndex, int owner, IEntityPool pool)
        {
            this.creationIndex = creationIndex;
            this.owner = owner;
            this.pool = pool;
            entity = null;
            contextType = pool.ObjectType;
        }

        public T GetComponent<T>() where T : IComponent
        {
            return Entity.GetComponent<T>();
        }

        public static implicit operator Entity(EntityReference ent)
        {
            return ent.Entity;
        }

        public static implicit operator EntityReference(Entity ent)
        { 
            return new EntityReference(ent);
        }

        public static bool operator ==(EntityReference reference, Entity ent)
        {
            if (ent == null)
                return reference.entity == ent || !reference.entity.Alive;
            return reference.creationIndex == ent.CreationIndex;
        }

        public static bool operator !=(EntityReference reference, Entity ent)
        {
            if (ent == null)
                return reference.entity != ent;
            return reference.creationIndex != ent.CreationIndex;
        }

        public void Rocketize(Rocketizer rocketizer, NetworkWriter writer)
        {
            writer.Write(creationIndex);
            writer.Write(rocketizer.GetIDFor(contextType));
        }

        public void DeRocketize(Rocketizer rocketizer, int ownerState, NetworkReader reader)
        {
            creationIndex = reader.ReadUInt32();
            owner = ownerState;
            contextType = rocketizer.GetTypeFor(reader.ReadInt16());
        }

        public void RocketizeReference(Rocketizer rocketizer)
        {
            throw new NotImplementedException();
        }
    }
}
