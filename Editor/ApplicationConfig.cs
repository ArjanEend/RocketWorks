﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace RocketWorks.Base
{
    [CreateAssetMenu]
    public class ApplicationConfig : ScriptableObject
    {
        [SerializeField] private List<ComponentConfig> components = new List<ComponentConfig>();
        public List<ComponentConfig> Components => components;

        [SerializeField] private List<ContextConfig> contexts = new List<ContextConfig>();
        public List<ContextConfig> Contexts => contexts;

    #if UNITY_EDITOR
        private void OnEnable()
        {
            for(int i = 0; i < contexts.Count; i++)
            {
                contexts[i].SetChoices(components);
            }
        }

        public void AddComponent(ComponentConfig newComponent)
        {
            components.Add(newComponent);
            newComponent.name = "New Component";
            string path = AssetDatabase.GetAssetPath(this);
            path = Path.GetDirectoryName(path);
            if(!Directory.Exists(path + "/Components_" + name))
                AssetDatabase.CreateFolder(path, "Components_" + name);
            AssetDatabase.CreateAsset(newComponent, path + "/Components_" + name + "/" + newComponent.name + ".asset");
        }

        public void RemoveComponent(ComponentConfig comp)
        {
            components.Remove(comp);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(comp));
        }

        public void GenerateCode()
        {
        }

        public void AddContext(ContextConfig newContext)
        {
            contexts.Add(newContext);
            newContext.name = "New Context";
            string path = AssetDatabase.GetAssetPath(this);
            path = Path.GetDirectoryName(path);
            if(!Directory.Exists(path + "/Contexts_" + name))
                AssetDatabase.CreateFolder(path, "Contexts_" + name);
            AssetDatabase.CreateAsset(newContext, path + "/Contexts_" + name + "/" + newContext.name + ".asset");
            newContext.SetChoices(components);
        }

        public void RemoveContext(ContextConfig context)
        {
            contexts.Remove(context);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(context));
        }
#endif
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(ApplicationConfig))]
    public class ApplicationConfigEditor : Editor
    {
        private Dictionary<Object, UnityEditor.Editor> cachedEditors
            = new Dictionary<Object, UnityEditor.Editor>();

        private Type[] componentTypes;

        private List<bool> foldEditors = new List<bool>();

        private void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            var config = (ApplicationConfig)target;
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Create Code"))
            {
                config.GenerateCode();
            }
            foreach(var comp in config.Components)
            {
                if(comp == null)
                {
                    config.RemoveComponent(comp);
                    return;
                }
                GUILayout.BeginHorizontal();
                //EditorGUILayout.LabelField(comp.GetType().Name);var fields = comp.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                EditorGUILayout.LabelField(comp.name);
                if(GUILayout.Button("-"))
                {
                    config.RemoveComponent(comp);
                    return;
                }

                GUILayout.EndHorizontal();
                DrawEditor(comp);
            }

            if (GUILayout.Button("Add component"))
            {
                var newComponent = ComponentConfig.CreateInstance<ComponentConfig>();
                config.AddComponent(newComponent);
            }

            GUILayout.Space(20);
            foreach(var context in config.Contexts)
            {
                if(context == null)
                {
                    config.RemoveContext(context);
                    return;
                }
                GUILayout.BeginHorizontal();
                //EditorGUILayout.LabelField(comp.GetType().Name);var fields = comp.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                EditorGUILayout.LabelField(context.name);
                if(GUILayout.Button("-"))
                {
                    config.RemoveContext(context);
                    return;
                }

                GUILayout.EndHorizontal();
                DrawEditor(context);
            }

            if (GUILayout.Button("Add context"))
            {
                var newContext = ContextConfig.CreateInstance<ContextConfig>();
                config.AddContext(newContext);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawEditor(Object obj)
        {
            EditorGUILayout.BeginVertical("box");
            UnityEditor.Editor editor = null;
            
            if (obj == null)
            {
                EditorGUILayout.LabelField(obj != null ? "loop: " + obj : "Back to start");
                EditorGUILayout.EndVertical();
                return;
            }
            
            if (cachedEditors.ContainsKey(obj))
            {
                editor = cachedEditors[obj];
            }

            CreateCachedEditor(obj, null, ref editor);

            cachedEditors[obj] = editor;
    
            if (editor != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                obj.name = EditorGUILayout.TextField(obj.name);
                if(GUILayout.Button("Rename"))
                {
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(obj), obj.name);
                }
                //obj.folded = EditorGUILayout.Toggle(obj.folded);
                EditorGUILayout.EndVertical();
                //if(!obj.folded)
                editor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
        }
    }
    #endif
}
