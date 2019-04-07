﻿using UnityEngine;
using System.Collections;
using RocketWorks.State;

namespace RocketWorks.Scene
{
    public abstract class SceneBase : StateBase<SceneHandler>
    {
        public delegate void OnPause(bool paused);

        public event OnPause onPause;
        private bool paused;

        public string sceneName = "";
        protected GameObject componentHolder;

        public SceneBase()
        {
            RocketLog.Log("Constructor", this );
            onPause = delegate { };

            paused = false;
        }

        public virtual void OnLoaded()
        {
            componentHolder = new GameObject(sceneName);
        }

        public virtual void Finish(SceneBase next)
        {
            OnFinish.Invoke(next);
            OnFinish = null;
            onPause = null;
        }

        public virtual void Pause()
        {
            paused = !paused;
            onPause(paused);
        }

    }
}