using System;

namespace RocketWorks.Commands
{
    public interface ICommand
    {
        void Execute(object target);
        Type targetType { get; }
    }

    public interface ICommand<T> : ICommand
    {
        void Execute(T target);
    }

}
