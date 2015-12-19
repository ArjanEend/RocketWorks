using System;

namespace RocketWorks.Loading
{
    interface ILoadable
    {
        float Progress { get; }
        void Cancel();
        void Load();
        event EventHandler onCompleteEvent;
        event EventHandler onFailEvent;
    }
}
