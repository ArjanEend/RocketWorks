using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace RocketWorks.Scene
{
    public class SceneHandler : MonoBehaviour
    {
        private SceneBase currentScene;
        private UnityEngine.SceneManagement.SceneManager unitySceneManager;

        private StateMachine<SceneHandler> stateMachine;

        private static SceneHandler instance;
        public static SceneHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    return Initialize();
                }
                else
                {
                    return instance;
                }
            }
        }

        public static SceneHandler Initialize()
        {
            if (instance != null)
                Debug.LogError("SceneManager exists already!");

            Debug.Log("New [SceneManager]");
            GameObject go = new GameObject("[SceneManager]");
            instance = go.AddComponent<SceneHandler>();
            DontDestroyOnLoad(go);
            DontDestroyOnLoad(instance);
            return instance;
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
            scene.onFinish += LoadScene;
            scene.onPause += HandlePause;
            currentScene = scene;
        }

        public void UnregisterScene(SceneBase scene)
        {
            scene.onFinish -= LoadScene;
            scene.onPause += HandlePause;
        }

        private void HandlePause(bool paused)
        {

        }

        private SceneBase LoadScene(StateBase<SceneHandler> scene)
        {
            SceneBase gScene = (SceneBase)scene;
            //and add the new one!
            RegisterScene(gScene);
            SceneManager.LoadScene(gScene.sceneName);
            //Application.LoadLevel(gScene.sceneName);
            return gScene;
        }

        private SceneBase LoadSceneWithLoader(StateBase<SceneHandler> scene)
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

    }
}
