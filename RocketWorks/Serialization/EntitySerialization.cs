using RocketWorks.Serialization;

namespace RocketWorks.Entities
{
    public partial class Entity : IRocketable
    {
        public void Rocketize(Rocketizer rocketizer)
        {

            RocketLog.Log("Entity rocketize!", this);
            rocketizer.Writer.Write(creationIndex);
            rocketizer.Writer.Write(Enabled);
            rocketizer.Writer.Write(Alive);
            
            for(int i = 0; i < components.Length; i++)
            {
                rocketizer.WriteObject(components[i]);
            }
            /*private uint creationIndex;

        private bool enabled;

        private bool alive;

        private IComponent[] components;

        private int composition;*/
        }

        public void DeRocketize(Rocketizer rocketizer)
        {

            RocketLog.Log("Entity rocketize!", this);
            creationIndex = rocketizer.Reader.ReadUInt32();
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
