using System;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class DrawInspectorAttribute : PropertyAttribute
{
    public DrawInspectorAttribute()
    {
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(DrawInspectorAttribute))]
    internal sealed class DrawInspectorDrawer : PropertyDrawer
    {
        private Dictionary<object, Editor> editors = new Dictionary<object, Editor>();

        private bool foldout = true;

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            foldout = EditorGUILayout.Toggle(foldout, GUILayout.MaxWidth(20));
            EditorGUILayout.PropertyField(property, GUILayout.MinWidth(200));
            EditorGUILayout.EndHorizontal();

            if (property.objectReferenceValue == null)
                return;

            EditorGUI.indentLevel++;

            Editor editor = null;
            if (editors.ContainsKey(property))
            {
                editor = editors[property];
            }
            else
            {
                Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
                editors[property] = editor;
            }

            if (foldout)
                editor.OnInspectorGUI();

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUI.indentLevel--;
        }
    }
}
#endif