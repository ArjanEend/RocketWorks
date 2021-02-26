using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.VersionControl;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace RocketWorks.Config
{
    public abstract class ConfigCollection : ScriptableObject
    {
        public abstract IList List { get; }
    }
    public class ConfigCollection<T> : ConfigCollection, IReadOnlyList<T>, IConfigCollection where T : ScriptableObject
    {
        [SerializeField]
        private List<T> configs = new List<T>();
    
        [NonSerialized]
        private IReadOnlyList<T> readOnlyConfigs;
        public IReadOnlyList<T> Configs => readOnlyConfigs ?? (readOnlyConfigs = configs.AsReadOnly());
    
        public override IList List => configs;
        public Type Type => typeof(T);
    
        public void AddAsset(object asset)
        {
            if (!(asset is T newItem))
                return;
            
            configs.Add(newItem);
            if(string.IsNullOrEmpty(newItem.name))
                newItem.name = $"{typeof(T).Name} {configs.Count}";
        }
        
        #if UNITY_EDITOR
        public void Clean()
        {
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
     
            foreach(Object o in objs)
            {
                if (o == this)
                    continue;
                if (configs.Contains(o as T))
                    continue;
                AssetDatabase.RemoveObjectFromAsset(o);
                DestroyImmediate(o);
            }
    
            var assets = AssetDatabase.FindAssets($"t: {typeof(T).Name}");
     
            foreach(string guid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var loadedAsset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (loadedAsset == null || AssetDatabase.IsSubAsset(loadedAsset))
                    continue;
    
                AssetDatabase.RemoveObjectFromAsset(loadedAsset);
                AssetDatabase.DeleteAsset(path);
                loadedAsset.hideFlags = HideFlags.None;
                AssetDatabase.AddObjectToAsset(loadedAsset, this);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(loadedAsset));
                AddAsset(loadedAsset);
            }
            
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
            AssetDatabase.Refresh();
        }
        #endif
        
        #region IReadOnlyList<T>
        public IEnumerator<T> GetEnumerator()
        {
            return Configs.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Configs).GetEnumerator();
        }
        public int Count => Configs.Count;
        public T this[int index] => Configs[index];
        #endregion
    }
    
    public interface IConfigCollection
    {
        IList List { get; }
        Type Type { get; }
    
        void AddAsset(object obj);
    #if UNITY_EDITOR
        void Clean();
    #endif
    }
}
