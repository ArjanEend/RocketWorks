using System;
using RocketWorks.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using RocketWorks.Pooling;
using RocketWorks.State;
using UnityScenes = UnityEngine.SceneManagement.SceneManager;
using RocketWorks.Controllers;

namespace RocketWorks.Scene
{
    [CreateAssetMenu]
    public class SceneHandler : UnitySystemBase
    {
        private SceneBase currentScene;
        private UnityEngine.SceneManagement.SceneManager unitySceneManager;

        private StateMachine<SceneHandler> stateMachine;

        public override void Initialize(Contexts contexts)
        {
            base.Initialize(contexts);
            stateMachine = new StateMachine<SceneHandler>(this);
            UnityScenes.sceneLoaded += OnSceneLoaded;
            RocketLog.Log("Start", this);
            OnSceneLoaded(UnityScenes.GetActiveScene(), LoadSceneMode.Single);
        }

        public void RegisterScene(SceneBase scene)
        {
            if (currentScene != null)
                UnregisterScene(currentScene);
            scene.OnFinish += LoadScene;
            scene.onPause += HandlePause;
            currentScene = scene;
        }

        public void UnregisterScene(SceneBase scene)
        {
            scene.OnFinish -= LoadScene;
            scene.onPause += HandlePause;
        }

        private void HandlePause(bool paused)
        {

        }

        private SceneBase LoadScene(IState<SceneHandler> scene)
        {
            SceneBase gScene = (SceneBase)scene;
            //and add the new one!
            RegisterScene(gScene);
            SceneManager.LoadScene(gScene.sceneName);
            return gScene;
        }

        private SceneBase LoadSceneWithLoader(IState<SceneHandler> scene)
        {
            SceneBase gScene = (SceneBase)scene;
            RegisterScene(gScene);

            SceneManager.LoadSceneAsync(gScene.sceneName);

            return gScene;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            var objects = scene.GetRootGameObjects();
            foreach(var obj in objects)
            {
                var controllers = obj.GetComponentsInChildren<IController>();
                foreach(var controller in controllers)
                {
                    controller.Init(contexts);
                }
            }
        }

        public override void Destroy()
        {
        }
    }
}
