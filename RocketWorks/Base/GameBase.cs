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

            float limitFrameTime = 1000f / 60f;
#if !UNITY
            do
            {
                Stopwatch FPSTimer = Stopwatch.StartNew();
                while (!Console.KeyAvailable)
                {
                    //Start of Tick
                    Stopwatch SW = Stopwatch.StartNew();

                    //The Actual Tick
                    systemManager.UpdateSystems();

                    //End of Tick
                    SW.Stop();
                    if (SW.Elapsed.TotalMilliseconds < limitFrameTime)
                    {
                        Thread.Sleep(Convert.ToInt32(limitFrameTime - SW.Elapsed.TotalMilliseconds));
                    }
                    else
                    {
                        Thread.Yield();
                    }
                }
            } while (true);
#endif
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
