using System;

namespace RocketWorks.Commands
{
    [Serializable]
    public abstract class CommandBase<T> : ICommand<T>
    {
        public Type targetType
        {
            get
            {
                return typeof(T);
            }
        }

        private int targetIndex;
        public int TargetIndex
        {
            get { return targetIndex; }
            set { TargetIndex = value; }
        }

        public abstract void Execute(T target);

        public virtual void Execute(object target)
        {
            T targetObj = (T)target;
            if(targetObj != null)
                Execute((T)target);
        }
    }

    [Serializable]
    public abstract class NetworkCommandBase<T> : CommandBase<T>, INetworkCommand<T>
    {
        public abstract void Execute(T target, int uid);
        public override void Execute(T target)
        {
            Execute(target, 0);
        }

        public void Execute(object target, int uid)
        {
            T targetObj = (T)target;
            if (targetObj != null)
                Execute((T)target, uid);
        }
    }
}
