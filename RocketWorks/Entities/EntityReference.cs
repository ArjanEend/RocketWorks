using RocketWorks.Pooling;
using RocketWorks.Serialization;
using System;
using System.IO;

namespace RocketWorks.Entities
{
    public partial struct EntityReference : IRocketable
    {
        public uint creationIndex;
        public int owner;

        public EntityPool pool;

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

        public EntityReference(uint creationIndex, int owner, EntityPool pool)
        {
            this.creationIndex = creationIndex;
            this.owner = owner;
            this.pool = pool;
            entity = null;
            contextType = pool.ObjectType;
        }

        public static implicit operator Entity(EntityReference ent)
        {
            return ent.pool.GetEntity(ent.creationIndex, ent.owner);
        }

        public static implicit operator EntityReference(Entity ent)
        { 
            return new EntityReference(ent);
        }

        public void Rocketize(Rocketizer rocketizer, BinaryWriter writer)
        {
            writer.Write(creationIndex);
            writer.Write(rocketizer.GetIDFor(contextType));
        }

        public void DeRocketize(Rocketizer rocketizer, int ownerState, BinaryReader reader)
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
