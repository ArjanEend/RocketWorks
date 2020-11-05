using System;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class NonEditableAttribute : PropertyAttribute
{
    public NonEditableAttribute()
    {
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(NonEditableAttribute))]
    internal sealed class NonEditableDrawer : PropertyDrawer
    {
        private static Dictionary<object, Editor> editors = new Dictionary<object, Editor>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property);
            GUI.enabled = true;
        }
    }
}
#endif