using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public abstract class Controller : ScriptableObject, IController
{
    private Action onInit = delegate { };
    public event Action OnInit
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

    public abstract void Init();
    public abstract void DeInit();

    private bool initialized = false;

    public bool Initialized => initialized;

    protected void OnInitialized()
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
    override public void Init()
    {
        // Empty
    }
    public abstract void Init(T item);
}

public interface IControllerInitialize<T>
{
    Task<T> DataPromise { get; }
}