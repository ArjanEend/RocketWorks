using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RocketWorks.Unity.DataDisplay;
using RocketWorks.Unity.Lists;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(RegisterToList))]
public class Window : MonoBehaviour, IPurePrefab
{
    [SerializeField]
    private WindowID windowId;
    public WindowID WindowId => windowId;

    [SerializeField]
    private LayerID layerId;

    [SerializeField, AutoRef]
    private GraphicRaycaster rayCaster;

    [SerializeField, NonEditable]
    private List<MonoBehaviour> windowListeners;

    [SerializeField, AutoRef, NonEditable]
    private Canvas canvas;

    [SerializeField]
    private int openDelay = 1;

    [SerializeField]
    private int closeDelay = 1;

    [SerializeField]
    private DisplayRoot displayRoot;

    public LayerID LayerId => layerId;

    private bool isOpen = false;

    public void CloseInstant()
    {
        gameObject?.SetActive(false);
        isOpen = false;
    }

    public async Task Close()
    {
        Unfocus();

        for (int i = 0; i < windowListeners.Count; i++)
        {
            if (windowListeners[i] is IOnWindowClose listener)
                listener.OnWindowClose();
        }

        if (displayRoot != null)
        {
            //displayRoot.Clear();
        }

        await Task.Delay(closeDelay);

        CloseInstant();
    }

    public async Task Open(object data = null)
    {
        if (isOpen)
            return;
        isOpen = true;
        if (data != null && displayRoot != null)
        {
            displayRoot.SetData(data);
            displayRoot.Display();
        }

        gameObject.SetActive(true);

        for (int i = 0; i < windowListeners.Count; i++)
        {
            if (windowListeners[i] is IOnWindowOpen listener)
                listener.OnWindowOpen();
        }

        Focus();

        await Task.Delay(openDelay);
    }

    public void Focus()
    {
        rayCaster.enabled = true;
        for (int i = 0; i < windowListeners.Count; i++)
        {
            if (windowListeners[i] is IOnWindowFocus listener)
                listener.OnWindowFocus();
        }
    }

    public void Unfocus()
    {
        rayCaster.enabled = false;
        for (int i = 0; i < windowListeners.Count; i++)
        {
            if (windowListeners[i] is IOnWindowUnfocus listener)
                listener.OnWindowUnfocus();
        }
    }

    public void CancelPressed()
    {
        for (int i = 0; i < windowListeners.Count; i++)
        {
            if (windowListeners[i] is IOnWindowBackPressed listener)
                listener.OnWindowBackPressed();
        }
    }

    private void OnValidate()
    {
        GetComponent<RegisterToList>().Behaviour = this;
        var newListeners = new List<MonoBehaviour>();
        List<MonoBehaviour> tempListeners = new List<MonoBehaviour>();
        tempListeners.AddRange(GetComponentsInChildren<IWindowBehaviour>().Select(x => x as MonoBehaviour));
        tempListeners = tempListeners.Distinct().ToList();

        if (!canvas.overrideSorting)
            canvas.overrideSorting = true;
        if (canvas.sortingOrder != layerId.sorting)
            canvas.sortingOrder = layerId.sorting;

        newListeners.AddRange(tempListeners);

        for (int i = 0; i < tempListeners.Count; i++)
        {
            if (i >= windowListeners.Count || !tempListeners[i].Equals(windowListeners[i]))
            {
#if UNITY_EDITOR
                windowListeners = tempListeners;
                UnityEditor.EditorUtility.SetDirty(this);
#endif
                return;
            }
        }
    }
}
