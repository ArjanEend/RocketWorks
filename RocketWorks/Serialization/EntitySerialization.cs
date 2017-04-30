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
            enabled = reader.ReadBoolean();
            alive = reader.ReadBoolean();
            for(int i = 0; i < components.Length; i++)
            {
                IComponent comp = rocketizer.ReadObject<IComponent>(reader);
                if(comp != null)
                    composition |= 1 << context(comp.GetType());
                components[i] = comp;
            }
        }

        public void RocketizeReference(Rocketizer rocketizer)
        {
            //writer.Writer.Write(creationIndex);
        }
    }
}
