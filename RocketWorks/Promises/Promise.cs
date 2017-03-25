using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketWorks.Promises
{
    public class Promise<T> : IPromise<T>
    {
        private Action<T> succesCallback;
        private Action<T> failCallback;
        private Action<T> completeCallback;
        
        public void Complete(T arg)
        {
            if (completeCallback != null)
                completeCallback(arg);
        }

        public void Fail(T arg)
        {
            if (failCallback != null)
                failCallback(arg);
        }

        public void Succeed(T arg)
        {
            if (succesCallback != null)
                succesCallback(arg);
        }

        public IPromise<T> OnComplete(Action<T> callback)
        {
            completeCallback = callback;
            return this;
        }

        public IPromise<T> OnFail(Action<T> callback)
        {
            failCallback = callback;
            return this;
        }

        public IPromise<T> OnSucces(Action<T> callback)
        {
            succesCallback = callback;
            return this;
        }
    }
}
