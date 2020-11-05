using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PurePrefabUtility
{
    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        EditorSceneManager.sceneDirtied += OnSceneDirtied;
        EditorSceneManager.sceneLoaded += OnSceneLoaded;
        EditorSceneManager.sceneSaving += OnSceneSave;
    }

    private static void OnSceneSave(Scene scene, string path)
    {
        OnSceneDirtied(scene);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        OnSceneDirtied(scene);
    }

    private static void OnSceneDirtied(Scene scene)
    {
        var GOs = scene.GetRootGameObjects();

        for (int i = 0; i < GOs.Length; i++)
        {
            if (Application.IsPlaying(GOs[i]))
                return;
            var purePrefabs = GOs[i].GetComponentsInChildren<IPurePrefab>();

            foreach (var purePrefab in purePrefabs)
            {
                var prefab = (MonoBehaviour) purePrefab;
                prefab.gameObject.hideFlags = HideFlags.NotEditable;
                if (prefab != null)
                {
                    PrefabUtility.RevertPrefabInstance(prefab.gameObject, InteractionMode.AutomatedAction);
                }
            }
        }
    }
}
