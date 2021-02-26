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
        private bool foldout = true;
        
        private static List<object> drawnObjects = new List<object>();

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var oldWidth = position.width;
            var oldHeight = position.height;
            position.height = 20;
            position.width = 20;
            foldout = EditorGUI.Foldout(position, foldout, "");
            position.x += 20;
            position.width = oldWidth -= 20;
            if(drawnObjects.Contains(property.objectReferenceValue))
                GUI.color = Color.yellow;
            EditorGUI.PropertyField(position, property);
            GUI.color = Color.white;
            position.x -= 20;
            position.width = oldWidth;
            position.height = oldHeight;
            position.y += 20;

            DrawerHeight = 16;

            if (property.objectReferenceValue == null)
                return;

            if (!foldout || drawnObjects.Contains(property.objectReferenceValue))
            {
                drawnObjects.Remove(property.objectReferenceValue);
                return;
            }

            drawnObjects.Add(property.objectReferenceValue);
            
            var indent = EditorGUI.indentLevel;
            position.x += 20;
            position.width -= 40;
            var e = Editor.CreateEditor(property.objectReferenceValue);
            var so = e.serializedObject;
            so.Update();

            var prop = so.GetIterator();
            prop.NextVisible(true);   
            
            int depthChilden = 0;
            bool showChilden = false;
            
            while (prop.NextVisible(prop.hasChildren && prop.isExpanded))
            {
                if (prop.depth == 0) { showChilden = false; depthChilden = 0; }
                if (showChilden && prop.depth > depthChilden)
                {
                    continue;
                }
                position.height = 16;
                EditorGUI.indentLevel = indent + prop.depth;
                if (EditorGUI.PropertyField(position, prop))
                {
                    showChilden = false;
                }
                else
                {
                    showChilden = true;
                    depthChilden = prop.depth;
                }

                var height = EditorGUI.GetPropertyHeight(prop);
                
                position.y += height;
                SetDrawerHeight(height);
            }
            
            drawnObjects.Remove(property.objectReferenceValue);
            if (GUI.changed)
                so.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
        } 
        
        void SetDrawerHeight(float height)
        {
            this.DrawerHeight += height;
        }

        public float DrawerHeight { get; set; }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);
            height += DrawerHeight;
            return height;
        }
    }
}
#endif