using RocketWorks.Systems;
using RocketWorks.Pooling;
using System.Diagnostics;
using System;
using System.Threading;

#if UNITY
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

        public virtual void UpdateFrame(float deltaTime)
        {
            systemManager.UpdateSystemsFrame(deltaTime);
        }
    }

#if UNITY
    public abstract class UnityGameBase : GameBase
    {
        protected SceneHandler sceneHandler;

        public UnityGameBase(): base()
        {
            UnitySystemBase.Create<SceneHandler>(systemManager);
            GameObject eventObject = new GameObject("[UpdateEvents]");
            UnityEvents events = eventObject.AddComponent<UnityEvents>();
            events.OnFixedUpdate += UpdateGame;
            events.OnUpdate += UpdateFrame;
        }
    }
#endif
}
