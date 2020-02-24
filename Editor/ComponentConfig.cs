#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;

namespace RocketWorks.Base
{
    [Serializable]
    public class VariableConfig
    {
        [SerializeField] public string typeName;
        [SerializeField] public string fieldName;

        public VariableConfig() { }
    }

    public interface IFoldable
    {
        bool Folded { get; set; }
    }

    public class ComponentConfig : ScriptableObject, IFoldable
    {
        [SerializeField] private List<VariableConfig> variables = new List<VariableConfig>();
        public List<VariableConfig> Variables => variables;

        private bool folded;
        public bool Folded { get => folded; set => folded = value; }

        public void AddVariable(VariableConfig variable)
        {
            variables.Add(variable);
        }

        public void RemoveVariable(VariableConfig variable)
        {
            variables.Remove(variable);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ComponentConfig))]
    public class VariableConfigEditor : Editor
    {

        private void OnEnable()
        {
        }

        private string inputText = "";

        public override void OnInspectorGUI()
        {
            var config = (ComponentConfig)target;
            EditorGUILayout.BeginVertical();

            foreach (var variable in config.Variables)
            {
                if (variable == null)
                {
                    config.RemoveVariable(variable);
                    return;
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("-"))
                {
                    config.RemoveVariable(variable);
                    return;
                }
                GUILayout.EndHorizontal();

                DrawEditor(variable);
            }

            EditorGUILayout.LabelField("Add variable");
            inputText = EditorGUILayout.TextField(inputText);
            // Update the selected choice in the underlying object
            //_choiceIndex = _choices[_choiceIndex];
            if (GUILayout.Button("Add"))
            {
                var component = new VariableConfig() { typeName = inputText, fieldName = "new" + inputText };
                config.AddVariable(component);
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawEditor(VariableConfig obj)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(20);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.EndVertical();

            obj.typeName = EditorGUILayout.TextField("type:", obj.typeName);
            obj.fieldName = EditorGUILayout.TextField("name:", obj.fieldName);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
    }
#endif
}