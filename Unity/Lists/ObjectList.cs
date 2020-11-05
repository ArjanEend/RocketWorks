using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RocketWorks.Unity.Lists
{
    public abstract class ObjectList : ScriptableObject
    {
        public abstract int Count { get; }
        public abstract void Add(object v);
        public abstract void Clear();
        public abstract void Remove(object v);
        public abstract void RemoveAt(int index);
    }

    public abstract class ObjectList<T> : ObjectList, IList<T>
    {
        private List<T> items = new List<T>();

        bool ICollection<T>.Remove(T item)
        {
            return items.Remove(item);
        }

        public override int Count => items.Count;
        public bool IsReadOnly => false;

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
    }
}