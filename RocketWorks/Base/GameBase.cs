using UnityEngine;
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
            entityPool = new EntityPool(typeof(TestComponent), typeof(PlayerIdComponent), typeof(AxisComponent));
            sceneHandler = UnitySystemBase.Initialize<SceneHandler>(entityPool);

            systemManager = new SystemManager(entityPool);
            systemManager.AddSystem(sceneHandler);

            GameObject eventObject = new GameObject("[UpdateEvents]");
            UnityEvents events = eventObject.AddComponent<UnityEvents>();
            events.OnUpdate += systemManager.UpdateSystems;
        }
    }
}
