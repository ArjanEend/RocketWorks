using RocketWorks.Serialization;
using System;
using System.IO;

namespace RocketWorks.Entities
{
    public partial struct EntityReference<T> : IRocketable where T : EntityContext
    {
        public uint creationIndex;
        public int owner;

        public EntityContext context;

        public Type ContextType { get { return typeof(T); } }

        public EntityReference(uint creationIndex, int owner)
        {
            this.creationIndex = creationIndex;
            this.owner = owner;
            context = null;
        }

        public EntityReference(uint creationIndex, int owner, EntityContext context)
        {
            this.creationIndex = creationIndex;
            this.owner = owner;
            this.context = context;
        }

        public static implicit operator Entity(EntityReference<T> ent)
        {
            return ent.context.Pool.GetEntity(ent.creationIndex, ent.owner);
        }

        public static implicit operator EntityReference<T>(Entity ent)
        {
            return new EntityReference<T>(ent.CreationIndex, ent.Owner);
        }

        public void Rocketize(Rocketizer rocketizer, BinaryWriter writer)
        {
            writer.Write(creationIndex);
            writer.Write(owner);
        }

        public void DeRocketize(Rocketizer rocketizer, BinaryReader reader)
        {
            creationIndex = reader.ReadUInt32();
            owner = reader.ReadInt32();
        }

        public void RocketizeReference(Rocketizer rocketizer)
        {
            throw new NotImplementedException();
        }
    }
}
