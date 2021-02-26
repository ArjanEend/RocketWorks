using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(99)]
public abstract class GameHookBase : MonoBehaviour
{
    [SerializeField]
    protected List<Controller> controllers;

    [SerializeField]
    protected Signal loadedEvent;

    [SerializeField] private bool checkDependencies = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!checkDependencies)
            return;

        var dependencies = UnityEditor.AssetDatabase.GetDependencies(gameObject.scene.path);

        for (int i = 0; i < dependencies.Length; i++)
        {
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath(dependencies[i], typeof(Controller)) as Controller;
            if (asset == null)
                continue;
            if (controllers.Contains(asset))
                continue;

            controllers.Add(asset);
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
    protected virtual void Start()
    {
        foreach (var controller in controllers)
        {
            controller.Init(this);
        }

        OnInit();

        loadedEvent.Emit();
    }

    public void InitData<T>(IController controller)
    {
        if (controller is IControllerInitialize<T> init)
        {
            init.DataPromise.ContinueWith(InitControllers);
        }
    }

    public abstract void OnInit();

    protected void DeInit()
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            controllers[i].DeInit();
        }
    }

    private void OnDestroy()
    {
        DeInit();
    }

    protected void InitControllers<T>(T value)
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            if (controllers[i] is IController<T> controller)
            {
                controller.Init(value);
            }
        }
    }
}
