using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RocketWorks.Unity.DataDisplay
{
    public class DisplayRoot : MonoBehaviour
    {
        private Dictionary<Type, List<IDataDisplay>> displayDictionary = new Dictionary<Type, List<IDataDisplay>>();
        private List<Type> typesToDisplay = new List<Type>();
        private object _data;
        public object Data => _data;

        [SerializeField, NonEditable]
        private List<MonoBehaviour> displays = new List<MonoBehaviour>();

        private bool initialized = false;

        public bool IsBoundTo(object value)
        {
            if (_data == null)
                return value == null;
            return _data.Equals(value);
        }

        private void OnValidate()
        {
            var newDisplays = new List<MonoBehaviour>();
            List<MonoBehaviour> tempDisplays = new List<MonoBehaviour>();
            tempDisplays.AddRange(GetComponentsInChildren<IDataDisplay>().Select(x => x as MonoBehaviour));

            for (int i = tempDisplays.Count - 1; i >= 0; i--)
            {
                if (tempDisplays[i].GetComponentInParent<DisplayRoot>() != this)
                {
                    tempDisplays.RemoveAt(i);
                }
            }

            newDisplays.AddRange(tempDisplays);

            for (int i = 0; i < tempDisplays.Count; i++)
            {
                if (i >= displays.Count || tempDisplays[i] != displays[i])
                {
#if UNITY_EDITOR
                    displays = tempDisplays;
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
                    return;
                }
            }
        }

        private void Awake()
        {
            if (initialized)
                return;
            initialized = true;

            for (int i = 0; i < displays.Count; i++)
            {
                IDataDisplay display = displays[i] as IDataDisplay;
                if (!displayDictionary.ContainsKey(display.Type))
                {
                    displayDictionary.Add(display.Type, new List<IDataDisplay>());
                    typesToDisplay.Add(display.Type);
                }

                displayDictionary[display.Type].Add(display);
            }
        }

        public void SetData(object data)
        {
            _data = data;
            Awake();

            for (int i = 0; i < typesToDisplay.Count; i++)
            {
                var displays = displayDictionary[typesToDisplay[i]];
                for (int j = 0; j < displays.Count; j++)
                {
                    var newData = data;
                    displays[j].SetData(data, out newData);
                }
            }
        }

        public void Display()
        {
            for (int i = 0; i < typesToDisplay.Count; i++)
            {
                var displays = displayDictionary[typesToDisplay[i]];
                for (int j = 0; j < displays.Count; j++)
                {
                    displays[j].Display();
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < displays.Count; i++)
            {
                if (displays[i] is IDataDisplay display)
                    display.Clear();
            }

            _data = null;
        }
    }

    public abstract class DataDisplayBase<T> : MonoBehaviour, IDataDisplay<T>
    {
        private T _data;
        protected T Data => _data;

        public Type Type => typeof(T);
        public bool SetData(object data, out object o)
        {
            Clear();
            o = data;
            if (data is T castData)
            {
                _data = castData;
                o = _data;
                OnSetData();
                return true;
            }

            var browseData = BrowseEncapsulationTree(o);
            if (browseData is T castBrowseData)
            {
                _data = castBrowseData;
                OnSetData();
                return true;
            }

            return false;
        }


        private T BrowseEncapsulationTree(object o)
        {
            if (o == null)
                return default(T);
            Type[] interfaces = o.GetType().GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                var interf = interfaces[i];
                if (!interf.IsGenericType || interf.GetGenericTypeDefinition() != typeof(IEncapsulates<>))
                    continue;

                PropertyInfo property = interf.GetProperty("EncapsulatedValue");
                if (property == null)
                    continue;

                object subObject = property.GetValue(o);
                if (subObject is T castObject)
                    return castObject;

                T result = BrowseEncapsulationTree(subObject);
                if (result != null)
                    return result;
            }

            return default(T);
        }


        public void Display()
        {
            if (ReferenceEquals(_data, null))
            {
                Debug.Log("This display didn't have any data...", this);
                return;
            }
            OnDisplay();
        }

        public virtual void OnSetData()
        {
            //
        }

        public abstract void OnDisplay();

        public void Clear()
        {
            if (!ReferenceEquals(_data, null))
                OnClear();
            _data = default(T);
        }

        protected virtual void OnClear()
        {
        }

        private void OnDestroy()
        {
            Clear();
        }
    }

    public interface IDataDisplay
    {
        Type Type { get; }

        bool SetData(object data, out object o);
        void Clear();

        void Display();
    }

    public interface IDataDisplay<T> : IDataDisplay
    {
    }

    public interface IEncapsulates<T>
    {
        T EncapsulatedValue { get; }
    }
}