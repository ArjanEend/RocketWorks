using UnityEngine;
using System.Collections;

namespace RocketWorks.Scene
{
    public abstract class GameScene : State<SceneManager>
    {
        public delegate void OnPause(bool paused);

        public event OnPause onPause;
        private bool paused;

        public string sceneName = "";
        protected GameObject componentHolder;

        public GameScene()
        {
            Debug.Log("New [Scene]");
            onPause = delegate { };

            paused = false;
        }

        public virtual void OnLoaded()
        {
            componentHolder = new GameObject(sceneName);
        }

        public virtual void Finish(GameScene next)
        {
            onFinish.Invoke(next);
            onFinish = null;
            onPause = null;
        }

        public virtual void Pause()
        {
            paused = !paused;
            onPause(paused);
        }

    }
}
