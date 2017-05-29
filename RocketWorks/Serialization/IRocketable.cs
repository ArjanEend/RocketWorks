using RocketWorks.Networking;

namespace RocketWorks.Serialization
{
    public interface IRocketable
    {
        void Rocketize(Rocketizer rocketizer, NetworkWriter writer);
        void DeRocketize(Rocketizer rocketizer, int ownerState, NetworkReader reader);
        void RocketizeReference(Rocketizer rocketizer);
    }
}
