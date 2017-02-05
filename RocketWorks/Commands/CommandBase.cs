﻿using System;

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

        public abstract void Execute(T target);

        public void Execute(object target)
        {
            T targetObj = (T)target;
            if(targetObj != null)
                Execute((T)target);
        }
    }
}