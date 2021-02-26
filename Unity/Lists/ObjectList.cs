using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RocketWorks.Unity.Lists
{
    public abstract class ObjectList : ScriptableObject, IExposedPropertyTable
    {
        public abstract void CopyTo(Array array, int index);
        public abstract int Count { get; }
        public abstract bool IsSynchronized { get; }
        public abstract object SyncRoot { get; }
        public abstract void Add(object v);

        public abstract void Clear();
        public abstract bool Contains(object value);
        public abstract int IndexOf(object value);
        public abstract void Insert(int index, object value);
        public abstract void Remove(object v);
        public abstract void RemoveAt(int index);
        public abstract bool IsFixedSize { get; }
        public abstract bool IsReadOnly { get; }

        public abstract void SetReferenceValue(PropertyName id, Object value);
        public abstract Object GetReferenceValue(PropertyName id, out bool idValid);
        public abstract void ClearReferenceValue(PropertyName id);
    }

    public abstract class ObjectList<T> : ObjectList, IList<T>, IList
    {
        private List<T> items = new List<T>();

        bool ICollection<T>.Remove(T item)
        {
            return items.Remove(item);
        }
        
        public override int Count => items.Count;
        public override bool IsReadOnly => false;
        object IList.this[int index] 
        {
            get => items[index];
            set => items[index] = (T)value;
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnPlayModeChanged(UnityEditor.PlayModeStateChange obj)
        {
            if (obj == UnityEditor.PlayModeStateChange.EnteredEditMode || obj == UnityEditor.PlayModeStateChange.ExitingEditMode)
                items.Clear();
        }
#endif

        private void OnDestroy()
        {
            items.Clear();
        }

        int IList.Add(object v)
        {
            Add(v);
            return Count;
        }
        public override void Add(object v)
        {
            if (v is T value)
            {
                Add(value);
                return;
            }

            throw new System.Exception($"{name} Add error, type is not correct! {v.GetType().Name}");
        }

        public override void Remove(object v)
        {
            if (v is T value)
            {
                Remove(value);
                return;
            }

            throw new System.Exception($"{name} Remove error, type is not correct! {v.GetType().Name}");
        }

        public override void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        public void Add(T value)
        {
            items.Add(value);
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public void Remove(T value)
        {
            items.Remove(value);
        }

        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            items.Insert(index, item);
        }

        public override void Clear()
        {
            items.Clear();
        }

        public T this[int index]
        {
            get => items[index];
            set => items[index] = value;
        }

        public T GetFirst()
        {
            if (items.Count == 0)
                return default(T);
            return items[0];
        }

        public T Find(Predicate<T> matcher)
        {
            return items.Find(matcher);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }

        public override void SetReferenceValue(PropertyName id, Object value)
        {
        }

        public override Object GetReferenceValue(PropertyName id, out bool idValid)
        {
            idValid = true;
            return null;
        }

        public override void ClearReferenceValue(PropertyName id)
        {
        }
        
        public override void CopyTo(Array array, int index)
        {
            ((IList) items).CopyTo(array, index);
        }

        public override bool IsSynchronized => ((IList) items).IsSynchronized;

        public override object SyncRoot => ((IList) items).SyncRoot;

        public override bool Contains(object value)
        {
            return ((IList) items).Contains(value);
        }

        public override int IndexOf(object value)
        {
            return ((IList) items).IndexOf(value);
        }

        public override void Insert(int index, object value)
        {
            ((IList) items).Insert(index, value);
        }

        public override bool IsFixedSize => ((IList) items).IsFixedSize;
    }
}