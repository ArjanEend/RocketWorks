using RocketWorks.Serialization;

namespace RocketWorks.Entities
{
    public partial class Entity : IRocketable
    {
        public void Rocketize(Rocketizer rocketizer)
        {
            rocketizer.Writer.Write(creationIndex);
            rocketizer.Writer.Write(composition);
            rocketizer.Writer.Write(Enabled);
            rocketizer.Writer.Write(Alive);
            for(int i = 0; i < components.Length; i++)
            {
                rocketizer.WriteObject(components[i]);
            }
        }

        public void DeRocketize(Rocketizer rocketizer)
        {
            creationIndex = rocketizer.Reader.ReadUInt32();
            composition = rocketizer.Reader.ReadInt32();
            enabled = rocketizer.Reader.ReadBoolean();
            alive = rocketizer.Reader.ReadBoolean();
            for(int i = 0; i < components.Length; i++)
            {
                components[i] = rocketizer.ReadObject<IComponent>();
            }
        }

        public void RocketizeReference(Rocketizer rocketizer)
        {
            rocketizer.Writer.Write(creationIndex);
        }
    }
}
