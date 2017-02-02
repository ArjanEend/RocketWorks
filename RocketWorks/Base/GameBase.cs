using RocketWorks.Systems;
using RocketWorks.Pooling;
using System.Diagnostics;
using System;
using System.Threading;

namespace RocketWorks.Base
{
    public abstract class GameBase
    {
        protected SystemManager systemManager;

        protected EntityPool entityPool;

        public GameBase()
        {
            entityPool = new EntityPool(typeof(PlayerIdComponent), typeof(MessageComponent));
            systemManager = new SystemManager(entityPool);
        }

        public virtual void UpdateGame()
        {
            systemManager.UpdateSystems();
        }
    }
}
#if UNITY
    using UnityEngine;
    using RocketWorks.Scene;

    public abstract class UnityGameBase : GameBase
    {
        protected SceneHandler sceneHandler;

        public UnityGameBase : base()
        {
            sceneHandler = UnitySystemBase.Initialize<SceneHandler>(entityPool);
            systemManager.AddSystem(sceneHandler);
            GameObject eventObject = new GameObject("[UpdateEvents]");
            UnityEvents events = eventObject.AddComponent<UnityEvents>();
            events.OnUpdate += systemManager.UpdateSystems;
        }
    }
#endif
