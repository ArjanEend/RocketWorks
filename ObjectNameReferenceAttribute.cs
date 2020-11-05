using System;
using UnityEngine;

[System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class ObjectNameReferenceAttribute : PropertyAttribute
{
    private Type type;
    public Type Type => type;

    public ObjectNameReferenceAttribute(Type t)
    {
        this.type = t;
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(ObjectNameReferenceAttribute))]
    public class ObjectNameRefDrawer : PropertyDrawer
    {

        private UnityEngine.Object foundObj = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var stringValue = property.stringValue;

            var objectNameRef = attribute as ObjectNameReferenceAttribute;

            if(foundObj == null)
                foundObj = Array.Find(UnityEngine.Object.FindObjectsOfType(objectNameRef.Type),
                    obj => obj.name == stringValue);

            var newObj = EditorGUI.ObjectField(position, label, foundObj, objectNameRef.Type);

            if (newObj != foundObj)
            {
                foundObj = newObj;
                property.stringValue = newObj?.name;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif