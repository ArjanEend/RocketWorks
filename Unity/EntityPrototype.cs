using System.Collections;
using System.Collections.Generic;
using RocketWorks.Entities;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.AddressableAssets;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
[CreateAssetMenu]
public class EntityPrototype : ScriptableObject
{
    [SerializeField]
    private List<string> components = new List<string>();
    
    [SerializeField]
    private List<string> types = new List<string>();

    private List<IComponent> cachedComponents = new List<IComponent>();
    public IList<IComponent> CachedComponents => cachedComponents;

    private void OnEnable()
    {
        for(int i = 0; i < components.Count; i++)
        {
            cachedComponents.Add(JsonUtility.FromJson(components[i], Type.GetType(types[i])) as IComponent);
        }
    }
    private void OnDisable()
    {
        for(int i = 0; i < cachedComponents.Count; i++)
        {
            components[i] = JsonUtility.ToJson(cachedComponents[i]);
        }
        EditorUtility.SetDirty(this);
    }

    private void OnBeforeSerialize()
    {
        OnDisable();
    }

    public void AddComponent(IComponent component)
    {
        this.types.Add(component.GetType().Name);
        this.components.Add(JsonUtility.ToJson(component));
        this.cachedComponents.Add(component);
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(EntityPrototype))]
public class EntityPrototypeEditor : Editor
{
    private ReorderableList list;
    private Type[] componentTypes;
    
    string[] choices = new [] {""};
    
    private int choiceIndex = -1;

    private void OnEnable()
    {
        list = new ReorderableList(this.serializedObject, serializedObject.FindProperty("component"));

        var componentTypes = Assembly.GetAssembly(this.GetType()).GetTypes().Where(x => typeof(IComponent).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract && !x.IsGenericType);
        choices = componentTypes.Select(x => x.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var prototype = (EntityPrototype)target;

        foreach(var comp in prototype.CachedComponents)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(comp.GetType().Name);
            var fields = comp.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            if(GUILayout.Button("-"))
            {

            }

            GUILayout.EndHorizontal();

            foreach(var field in fields)
            {
                if(field.FieldType.IsSubclassOf(typeof(AssetReference)))
                {
                    AssetReference assetRef = field.GetValue(comp) as AssetReference;
                    assetRef.SetEditorAsset(EditorGUILayout.ObjectField(assetRef.editorAsset, typeof(GameObject)));
                } else if(typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType)) {
                    field.SetValue(comp, EditorGUILayout.ObjectField(field.GetValue(comp) as UnityEngine.Object, field.FieldType));
                } else if(field.FieldType == typeof(Vector3))
                {
                    field.SetValue(comp, EditorGUILayout.Vector3Field(field.Name, (Vector3)field.GetValue(comp)));
                }
            }
        }

        EditorGUILayout.LabelField("Add component");
        choiceIndex = EditorGUILayout.Popup(choiceIndex, choices);
        // Update the selected choice in the underlying object
        //_choiceIndex = _choices[_choiceIndex];
        if(choiceIndex != -1)
        {
            var component = Activator.CreateInstance(Type.GetType(choices[choiceIndex]));
            prototype.AddComponent(component as IComponent);
            EditorUtility.SetDirty(target);
        }
        choiceIndex = -1;

    }
}
#endif