using System.IO;

namespace RocketWorks.Serialization
{
    public interface IRocketable
    {
        void Rocketize(Rocketizer rocketizer, BinaryWriter writer);
        void DeRocketize(Rocketizer rocketizer, BinaryReader reader);
        void RocketizeReference(Rocketizer rocketizer);
    }
}
