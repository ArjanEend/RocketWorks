

namespace RocketWorks.Serialization
{
    public interface IRocketable
    {
        void Rocketize(Rocketizer rocketizer);
        void DeRocketize(Rocketizer rocketizer);
        void RocketizeReference(Rocketizer rocketizer);
    }
}
