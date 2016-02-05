using UnityEngine;
using System.Collections;
using RocketWorks.Scene;
using RocketWorks.Systems;
using RocketWorks.Pooling;

namespace RocketWorks.Base
{
    public class GameBase
    {
        protected SceneHandler sceneManager;
        protected SystemManager systemManager;

        private EntityPool entityPool;

        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeInitialize()
        {
            Debug.Log("[GameBase] Called from RuntimeInitialize!");
            new GameBase();
        }

        public GameBase()
        {
            sceneManager = UnitySystemBase.Initialize<SceneHandler>();

            entityPool = new EntityPool();

            systemManager = new SystemManager();
            systemManager.AddSystem(sceneManager);
        }
    }
}
