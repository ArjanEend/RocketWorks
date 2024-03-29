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
    public class ContextConfig : ScriptableObject
    {
        public List<ComponentConfig> components = new List<ComponentConfig>();

        [SerializeField]
        private List<ComponentConfig> componentChoices = new List<ComponentConfig>();
        public List<ComponentConfig> ComponentChoices => componentChoices;

#if UNITY_EDITOR
        public void SetChoices(List<ComponentConfig> configs)
        {
            componentChoices = configs;
        }

        public void AddComponent(ComponentConfig newComponent)
        {
            components.Add(newComponent);

            EditorUtility.SetDirty(this);
        }

        public void RemoveComponent(ComponentConfig comp)
        {
            components.Remove(comp);

            EditorUtility.SetDirty(this);
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ContextConfig))]
    public class ContextConfigEditor : Editor
    {
        private int choiceIndex = -1;

        private void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            var config = (ContextConfig)target;
            EditorGUILayout.BeginVertical();

            foreach (var comp in config.components)
            {
                if (comp == null)
                {
                    config.RemoveComponent(comp);
                    return;
                }
                GUILayout.BeginHorizontal();
                //EditorGUILayout.LabelField(comp.GetType().Name);var fields = comp.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                EditorGUILayout.LabelField(comp.name);
                if (GUILayout.Button("-"))
                {
                    config.RemoveComponent(comp);
                    return;
                }

                GUILayout.EndHorizontal();
            }

            var choices = config.ComponentChoices
                .Where(_ => !config.components.Contains(_))
                .ToDictionary(_ => _.name, _ => _);

            EditorGUILayout.LabelField("Add component");
            choiceIndex = EditorGUILayout.Popup(choiceIndex, choices.Keys.ToArray());
            // Update the selected choice in the underlying object
            //_choiceIndex = _choices[_choiceIndex];
            if (choiceIndex != -1)
            {
                config.AddComponent(choices[choices.Keys.ElementAt(choiceIndex)]);
                EditorUtility.SetDirty(target);
            }
            choiceIndex = -1;
            EditorGUILayout.EndVertical();
        }
    }
#endif
}
