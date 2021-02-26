using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;

#endif

namespace RocketWorks
{
    [System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class AutoRefAttribute : PropertyAttribute
    {
        public AutoRefAttribute()
        {
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AutoRefAttribute))]
    public class AutoRefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property);

            if (property.objectReferenceValue != null)
                return;

            var objType = property.serializedObject.targetObject.GetType();
            FieldInfo field = null;
            while (field == null && objType.BaseType != null)
            {
                field = objType.GetField(property.propertyPath, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                objType = objType.BaseType;
            }
            var propertyType = field.FieldType;

            if (propertyType.IsSubclassOf(typeof(Behaviour)))
            {
                var monoBehaviour = (Behaviour)property.serializedObject.targetObject;

                var foundBehaviour = monoBehaviour.GetComponentInChildren(this.fieldInfo.FieldType);
                if (property.objectReferenceValue == foundBehaviour)
                    return;

                property.objectReferenceValue = foundBehaviour;
                property.serializedObject.ApplyModifiedProperties();
            }
            if (propertyType.IsSubclassOf(typeof(ScriptableObject)))
            {
                var filter = $"t:{propertyType.Name}";
                var paths = AssetDatabase.FindAssets(filter);

                if (paths.Length == 0)
                    return;

                var firstFoundAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[0]), propertyType);

                if (property.objectReferenceValue == firstFoundAsset)
                    return;

                property.objectReferenceValue = firstFoundAsset;
                property.serializedObject.ApplyModifiedProperties();
            }

        }
    }
#endif
}