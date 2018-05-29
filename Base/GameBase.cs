using RocketWorks.Systems;
using RocketWorks.Pooling;
using System.Diagnostics;
using System;
using System.Threading;

#if RW_UNITY
using UnityEngine;
using RocketWorks.Scene;
#endif

namespace RocketWorks.Base
{
    public abstract class GameBase
    {
        protected SystemManager systemManager;

        protected Contexts contexts;

        public GameBase()
        {
            contexts = new Contexts();
            systemManager = new SystemManager(contexts);
        }

        public virtual void UpdateGame(float deltaTime)
        {
            systemManager.UpdateSystems(deltaTime);
        }
    }

#if RW_UNITY
    public abstract class UnityGameBase : GameBase
    {
        protected SceneHandler sceneHandler;

        public UnityGameBase(): base()
        {
            sceneHandler = UnitySystemBase.Create<SceneHandler>();
            systemManager.AddSystem(sceneHandler);
            GameObject eventObject = new GameObject("[UpdateEvents]");
            UnityEvents events = eventObject.AddComponent<UnityEvents>();
            events.OnUpdate += UpdateGame;
        }
    }
#endif
}
