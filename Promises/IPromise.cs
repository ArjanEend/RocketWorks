using System;
using System.Collections.Generic;

namespace RocketWorks.Promises
{
    public interface IPromise<T>
    {
        IPromise<T> OnSucces(Action<T> callback);
        IPromise<T> OnFail(Action<T> callback);
        IPromise<T> OnComplete(Action<T> callback);

        void Succeed(T arg);
        void Fail(T arg);
        void Complete(T arg);
    }
}
