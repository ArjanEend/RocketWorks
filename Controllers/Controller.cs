using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public abstract class Controller : ScriptableObject, IController
{
    private Action onInit = delegate { };
    public event Action OnInitialized
    {
        add
        {
            if (initialized)
            {
                value();
            }
            onInit += value;
        }
        remove { onInit -= value; }
    }

    public void Init(GameHookBase bootStrap)
    {
        initialized = false;
        OnInit(bootStrap);
    }

    public abstract void OnInit(GameHookBase bootStrap);
    public abstract void DeInit();

    private bool initialized = false;

    public bool Initialized => initialized;

    protected void InitializeComplete()
    {
        initialized = true;
        onInit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeChange;
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeChange;
#endif
    }

#if UNITY_EDITOR
    private void OnPlayModeChange(PlayModeStateChange obj)
    {
        initialized = false;
    }
#endif
}

public abstract class Controller<T> : Controller, IController<T>
{
    override public void OnInit(GameHookBase bootStrapper)
    {
        // Empty
    }
    public abstract void Init(T item);
}

public interface IControllerInitialize<T>
{
    Task<T> DataPromise { get; }
}
