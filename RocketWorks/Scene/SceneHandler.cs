using System;
using RocketWorks.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using RocketWorks.Pooling;
using RocketWorks.State;

namespace RocketWorks.Scene
{
    public class SceneHandler : UnitySystemBase
    {
        private SceneBase currentScene;
        private UnityEngine.SceneManagement.SceneManager unitySceneManager;

        private StateMachine<SceneHandler> stateMachine;

        public override void Initialize(EntityPool pool)
        {
            
        }

        private void Start()
        {
            stateMachine = new StateMachine<SceneHandler>(this);

            Debug.Log("[SceneManager] Start");
        }

        void Update()
        {
            stateMachine.Update();
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
            //Application.LoadLevel(gScene.sceneName);
            return gScene;
        }

        private SceneBase LoadSceneWithLoader(IState<SceneHandler> scene)
        {
            SceneBase gScene = (SceneBase)scene;
            RegisterScene(gScene);

            SceneManager.LoadSceneAsync(gScene.sceneName);

            return gScene;
        }

        void OnLevelWasLoaded()
        {
            currentScene.OnLoaded();
        }

        public override void Execute()
        {
            //throw new NotImplementedException();
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
