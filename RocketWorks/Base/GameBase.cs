using RocketWorks.Systems;
using RocketWorks.Pooling;
using System.Diagnostics;
using System;
using System.Threading;
using Implementation.Components;

#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
using RocketWorks.Scene;
#endif

namespace RocketWorks.Base
{
    public abstract class GameBase
    {
        protected SystemManager systemManager;

        protected EntityPool entityPool;

        public GameBase()
        {
            entityPool = new EntityPool(typeof(PlayerIdComponent), typeof(MessageComponent), typeof(VisualizationComponent), typeof(MovementComponent), typeof(TransformComponent));
            systemManager = new SystemManager(entityPool);
        }

        public virtual void UpdateGame(float deltaTime)
        {
            systemManager.UpdateSystems(deltaTime);
        }
    }

#if UNITY_EDITOR || UNITY_STANDALONE
    public abstract class UnityGameBase : GameBase
    {
        protected SceneHandler sceneHandler;

        public UnityGameBase(): base()
        {
            sceneHandler = UnitySystemBase.Initialize<SceneHandler>(entityPool);
            systemManager.AddSystem(sceneHandler);
            GameObject eventObject = new GameObject("[UpdateEvents]");
            UnityEvents events = eventObject.AddComponent<UnityEvents>();
            events.OnUpdate += UpdateGame;
        }
    }

#endif
}
