using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RocketWorks
{
    [System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class NonEditableAttribute : PropertyAttribute
    {
        public NonEditableAttribute()
        {
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(NonEditableAttribute))]
    public class NonEditableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property);
            GUI.enabled = true;
        }
    }
#endif
}