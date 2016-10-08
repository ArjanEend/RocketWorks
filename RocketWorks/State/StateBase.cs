﻿using System;
namespace RocketWorks.State
{
    public abstract class StateBase<T> : IState<T>
    {
        protected T entity;
        public StateFinished onFinish;

        public void RegisterState(T actor)
        {
            this.entity = actor;
        }

        public abstract void Initialize();
        public abstract void Exit();
        public abstract void Update();
        public abstract void FixedUpdate();

        public void DispatchFinishEvent(IState next)
        {
            if (onFinish != null)
                onFinish(next);
        }
    }
}