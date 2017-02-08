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
        void Execute(object target, uint uid);
    }

    public interface ICommand<T> : ICommand
    {
        void Execute(T target);
    }

    public interface INetworkCommand<T> : INetworkCommand
    {
        void Execute(T target, uint uid);
    }

}
