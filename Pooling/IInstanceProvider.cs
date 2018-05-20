using System;

namespace RocketWorks.Pooling
{
    public interface IInstanceProvider
    {
        object GetInstance();
        Type ObjectType { get; }
    }

    public interface IInstanceProvider<T> : IInstanceProvider
    {
        T GetTypedInstance();
    }
}
