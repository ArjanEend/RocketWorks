namespace RocketWorks.Entities
{
    interface IEstimatable
    {
        void Estimate(object against, float deltaTime, bool local);
    }

    interface IEstimatable<T> : IEstimatable
    {
        void Estimate(T against, float deltaTime, bool local);
    }
}
