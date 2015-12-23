using UnityEngine;
using System.Collections;
using RocketWorks.Scene;

namespace RocketWorks.Base
{
    public class GameBase
    {
        private SceneHandler sceneManager;

        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeInitialize()
        {
            Debug.Log("[GameBase] Called from RuntimeInitialize!");
            new GameBase();
        }

        public GameBase()
        {
            sceneManager = SceneHandler.Initialize();
        }
    }
}
