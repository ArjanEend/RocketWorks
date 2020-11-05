using System;
using System.Reflection;
using UnityEngine;

[System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class AutoRefAttribute : PropertyAttribute
{

    public AutoRefAttribute()
    {
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(AutoRefAttribute))]
    internal sealed class AutoRefDrawer : PropertyDrawer
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

            if (propertyType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                var monoBehaviour = (MonoBehaviour)property.serializedObject.targetObject;

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
}
#endif
