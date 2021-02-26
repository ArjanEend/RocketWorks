
using System;
using System.Collections.Generic;
using RocketWorks.Unity.Lists;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RocketWorks
{
    [CreateAssetMenu(menuName = "RocketWorks/Controllers/Window", fileName = "WindowController")]
    public class WindowController : Controller
    {
        public event Action<WindowID> OnWindowClosed = delegate { };
        public event Action<WindowID> OnWindowOpened = delegate { };
        public event Action<WindowID> OnWindowFocused = delegate { };
        public event Action<LayerID> OnLayerChanged = delegate { };

        [SerializeField]
        private WindowList windows;

        [SerializeField]
        private List<LayerID> layers = new List<LayerID>();

        private Dictionary<WindowID, Window> windowBindings = new Dictionary<WindowID, Window>();

        private Dictionary<LayerID, Stack<Window>> layerWindowStack = new Dictionary<LayerID, Stack<Window>>();

        [SerializeField]
        private Queue<WindowID> pendingWindows = new Queue<WindowID>();

        [SerializeField]
        private Queue<object> pendingData = new Queue<object>();

        [SerializeField]
        private Signal bootstrapperEvent;

        [SerializeField]
        private InputActionReference backAction;

        [SerializeField] 
        private WindowID initialWindow;

        private LayerID currentTopLayer = null;

        private bool isTransitioning = false;
        public bool IsTransitioning => isTransitioning;

        public override void OnInit(GameHookBase bootStrap)
        {
            for (int i = 0; i < windows.Count; i++)
            {
                windowBindings.Add(windows[i].WindowId, windows[i]);
                windows[i].CloseInstant();
            }

            for (int i = 0; i < layers.Count; i++)
            {
                layerWindowStack.Add(layers[i], new Stack<Window>());
            }

            bootstrapperEvent.OnEmit += OnBootstrapDone;

            backAction.action.performed += OnBackPressed;
        }

        private void OnBootstrapDone()
        {
            Open(initialWindow);
        }

        private void OnBackPressed(InputAction.CallbackContext obj)
        {
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                var layer = layers[i];
                if (layerWindowStack[layer].Count == 0)
                    continue;
                layerWindowStack[layer].Peek().CancelPressed();
                return;
            }
        }

        public override void DeInit()
        {
            windowBindings.Clear();
            layerWindowStack.Clear();
            pendingWindows.Clear();
            pendingData.Clear();
            isTransitioning = false;

            
            bootstrapperEvent.OnEmit -= OnBootstrapDone;
            backAction.action.performed -= OnBackPressed;
        }

        public async void Open(WindowID id, object data = null)
        {
            if (isTransitioning)
            {
                pendingWindows.Enqueue(id);
                pendingData.Enqueue(data);
                return;
            }
            isTransitioning = true;

            if (!windowBindings.ContainsKey(id))
            {
                Debug.Log($"Window {id.name} doesn't exist right now!");
                return;
            }

            var window = windowBindings[id];

            int index = layers.IndexOf(window.LayerId);
            for (int i = layers.Count - 1; i >= index; i--)
            {
                var layer = layers[i];
                if (layerWindowStack[layer].Count == 0)
                    continue;
                await layerWindowStack[layer].Peek().Close();
                OnWindowClosed(layerWindowStack[layer].Peek().WindowId);
                if (i != index)
                    layerWindowStack[layer].Pop();
            }

            for (int i = 0; i < index; i++)
            {
                if (layerWindowStack[layers[i]].Count > 0)
                    layerWindowStack[layers[i]].Peek().Unfocus();
            }

            if (currentTopLayer != window.LayerId)
            {
                currentTopLayer = window.LayerId;
                OnLayerChanged(currentTopLayer);
            }

            layerWindowStack[window.LayerId].Push(window);
            await window.Open(data);
            OnWindowOpened(window.WindowId);
            isTransitioning = false;
            if (pendingWindows.Count > 0)
            {
                Open(pendingWindows.Dequeue(), pendingData.Dequeue());
            }
        }

        public async void CloseCurrent()
        {
            isTransitioning = true;
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                var layer = layers[i];
                if (layerWindowStack[layers[i]].Count == 0)
                    continue;
                var window = layerWindowStack[layer].Pop();
                await window.Close();
                OnWindowClosed(window.WindowId);

                //If the current layer stack is empty, move down a layer, i.e. move back to gameplay
                if (layerWindowStack[layer].Count == 0)
                {
                    while (i >= 0 && layerWindowStack[layers[i]].Count == 0)
                    {
                        i--;
                        continue;
                    }

                    var newTopWindow = layerWindowStack[layers[i]].Peek();
                    newTopWindow.Focus();
                    currentTopLayer = layers[i];
                    OnLayerChanged(currentTopLayer);
                    OnWindowFocused(newTopWindow.WindowId); isTransitioning = false;
                    if (pendingWindows.Count > 0)
                    {
                        Open(pendingWindows.Dequeue(), pendingData.Dequeue());
                    }
                    return;
                }

                //Open previous window
                await layerWindowStack[layer].Peek().Open();
                OnWindowOpened(layerWindowStack[layer].Peek().WindowId);

                isTransitioning = false;
                if (pendingWindows.Count > 0)
                {
                    Open(pendingWindows.Dequeue(), pendingData.Dequeue());
                }

                return;
            }
        }
    }
}
