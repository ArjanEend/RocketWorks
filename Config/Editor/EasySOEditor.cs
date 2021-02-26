using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RocketWorks.Config
{
    [CustomPropertyDrawer(typeof(ScriptableObject), true)]
    public class EasySOEditor : PropertyDrawer
    {
        private bool initialized = false;
        private CreateSOMenu menu;

        private SerializedProperty prop;
        private bool rename = false;

        private void Init(SerializedProperty prop)
        {
            if (initialized)
                return;
            initialized = true;

            Type parentType = prop.serializedObject.targetObject.GetType();
            FieldInfo field = null;
            var path = prop.propertyPath.Contains(".")
                ? prop.propertyPath.Substring(0, prop.propertyPath.IndexOf('.'))
                : prop.propertyPath;
            while (field == null && parentType.BaseType != null)
            {
                field = parentType.GetField(path, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                parentType = parentType.BaseType;
            }

            var type = field.FieldType;
            if (type.GetElementType() is Type newType)
                type = newType;
            if (type.GetInterfaces().Contains(typeof(IList)))
                type = type.GetGenericArguments()[0];

            this.prop = prop;

            menu = new CreateSOMenu(type, OnTypeCreate);
        }

        private void OnTypeCreate(object userdata)
        {
            var type = userdata as Type;

            var collectionType = type.Assembly.GetTypes().Where(t =>
                t.BaseType != null && t.BaseType.IsGenericType &&
                t.BaseType.GetGenericTypeDefinition() == typeof(ConfigCollection<>)).Where((t) =>
            {
                var genericArg = t.BaseType.GetGenericArguments()[0];
                return genericArg == type || type.IsSubclassOf(genericArg);
            }).FirstOrDefault();

            if (collectionType == null)
            {
                var newAsset = ScriptableObject.CreateInstance(type);
                newAsset.name = type.Name;

                string prepend = "";
                var baseType = type;
                while (baseType != typeof(ScriptableObject))
                {
                    if (baseType.BaseType.IsGenericType)
                        prepend = type.BaseType.GetGenericTypeDefinition().Name + "/" + prepend;
                    else
                        prepend = type.BaseType.Name + "/" + prepend;
                    baseType = baseType.BaseType;
                }

                prepend = prepend.Replace("`1", "");
                prepend = prepend.Replace("`2", "");

                var assetPath = $"Assets/Configs/{prepend}{newAsset.name}.asset";
                Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
                AssetDatabase.CreateAsset(newAsset, assetPath);
                prop.objectReferenceValue = newAsset;
                return;
            }

            var assetsFound = AssetDatabase.FindAssets($"t:{collectionType.Name}");

            ScriptableObject collectionAsset = null;
            if (assetsFound.Length == 0)
            {
                var newCollection = ScriptableObject.CreateInstance(collectionType.Name);
                newCollection.name = collectionType.Name;
                var assetPath = $"Assets/Configs/{newCollection.name}.asset";
                Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
                AssetDatabase.CreateAsset(newCollection, assetPath);
                collectionAsset = newCollection;
            }
            else
            {
                collectionAsset =
                    AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(assetsFound.First()));
            }

            var collection = collectionAsset as IConfigCollection;

            var asset = ScriptableObject.CreateInstance(type);
            asset.hideFlags = HideFlags.None;
            AssetDatabase.AddObjectToAsset(asset, collectionAsset);
            collection.AddAsset(asset);
            prop.objectReferenceValue = asset;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.width -= 120;

            if (rename)
            {
                property.objectReferenceValue.name = EditorGUI.TextField(position, property.objectReferenceValue.name);
            }
            else
            {
                EditorGUI.PropertyField(position, property, true);
            }

            position.x += position.width;
            position.width = 40;
            if (GUI.Button(position, EditorGUIUtility.IconContent("InputField Icon")))
            {
                rename = !rename;
            }

            position.x += 40;
            if (GUI.Button(position, EditorGUIUtility.IconContent("d_CreateAddNew")))
            {
                Init(property);
                menu.Show();
            }

            position.x += 40;
            if (property.objectReferenceValue != null &&
                GUI.Button(position, EditorGUIUtility.IconContent("d_winbtn_win_restore_a@2x")))
            {
                var window = EditSOWindow.CreateWindow<EditSOWindow>();
                window.Show(property.objectReferenceValue);
            }
        }
    }

    public class EditSOWindow : EditorWindow
    {
        private Object targetObject;

        private Editor editor;

        public void Show(Object so)
        {
            targetObject = so;
            Editor.CreateCachedEditor(targetObject, null, ref editor);
            ShowPopup();
        }

        private void OnGUI()
        {
            editor.OnInspectorGUI();
        }
    }
}