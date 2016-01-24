using UnityEngine;
using System.Collections;
using RocketWorks.Scene;
using RocketWorks.System;

namespace RocketWorks.Base
{
    public class GameBase
    {
        protected SceneHandler sceneManager;
        protected SystemManager systemManager;

        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeInitialize()
        {
            Debug.Log("[GameBase] Called from RuntimeInitialize!");
            new GameBase();
        }

        public GameBase()
        {
            sceneManager = UnitySystemBase.Initialize<SceneHandler>();

            systemManager = new SystemManager();
            systemManager.AddSystem(sceneManager);
        }
    }
}
