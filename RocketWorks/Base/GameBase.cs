using UnityEngine;
using System.Collections;
using RocketWorks.Scene;
using RocketWorks.Systems;
using RocketWorks.Pooling;

namespace RocketWorks.Base
{
    public abstract class GameBase
    {
        protected SceneHandler sceneHandler;
        protected SystemManager systemManager;

        protected EntityPool entityPool;

        public GameBase()
        {
            sceneHandler = UnitySystemBase.Initialize<SceneHandler>();

            entityPool = new EntityPool();

            systemManager = new SystemManager(entityPool);
            systemManager.AddSystem(sceneHandler);
        }
    }
}
