using UnityEngine;
using System.Collections;

namespace RocketWorks.Scene
{
    public class SceneManager : MonoBehaviour
    {
        private GameScene currentScene;

        private StateMachine<SceneManager> stateMachine;

        private static SceneManager instance;
        public static SceneManager Instance
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

        public static SceneManager Initialize()
        {
            if (instance != null)
                Debug.LogError("SceneManager exists already!");

            Debug.Log("New [SceneManager]");
            GameObject go = new GameObject("[SceneManager]");
            instance = go.AddComponent<SceneManager>();
            DontDestroyOnLoad(go);
            DontDestroyOnLoad(instance);
            return instance;
        }

        private void Start()
        {
            stateMachine = new StateMachine<SceneManager>(this);

            Debug.Log("[SceneManager] Start");
        }

        void Update()
        {
            stateMachine.Update();
        }

        public void RegisterScene(GameScene scene)
        {
            scene.onFinish += LoadScene;
            scene.onPause += HandlePause;
            currentScene = scene;
        }

        private void HandlePause(bool paused)
        {

        }

        private GameScene LoadScene(State<SceneManager> scene)
        {
            GameScene gScene = (GameScene)scene;
            //and add the new one!
            RegisterScene(gScene);
            Application.LoadLevel(gScene.sceneName);
            return gScene;
        }

        void OnLevelWasLoaded()
        {
            currentScene.OnLoaded();
        }

    }
}
