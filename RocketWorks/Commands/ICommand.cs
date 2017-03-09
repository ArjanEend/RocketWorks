using System;

namespace RocketWorks.Commands
{
    public interface ICommand
    {
        void Execute(object target);
        Type targetType { get; }
    }

    public interface INetworkCommand : ICommand
    {
        void Execute(object target, int uid);
    }

    public interface ICommand<T> : ICommand
    {
        void Execute(T target);
    }

    public interface INetworkCommand<T> : INetworkCommand
    {
        void Execute(T target, int uid);
    }

}
