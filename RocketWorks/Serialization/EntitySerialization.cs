using System;
using RocketWorks.Serialization;
using System.IO;

namespace RocketWorks.Entities
{
    public partial class Entity : IRocketable
    {
        public void Rocketize(Rocketizer rocketizer, BinaryWriter writer)
        {
            writer.Write(creationIndex);
            writer.Write(composition);
            writer.Write(Enabled);
            writer.Write(Alive);
            for(int i = 0; i < components.Length; i++)
            {
                rocketizer.WriteObject(components[i], writer);
            }
        }

        public void DeRocketize(Rocketizer rocketizer, BinaryReader reader)
        {
            creationIndex = reader.ReadUInt32();
            composition = reader.ReadInt32();
            enabled = reader.ReadBoolean();
            alive = reader.ReadBoolean();
            for(int i = 0; i < components.Length; i++)
            {
                components[i] = rocketizer.ReadObject<IComponent>(reader);
            }
        }

        public void RocketizeReference(Rocketizer rocketizer)
        {
            //rocketizer.Writer.Write(creationIndex);
        }
    }
}
