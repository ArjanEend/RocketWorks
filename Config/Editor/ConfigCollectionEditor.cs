using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = System.Object;

namespace RocketWorks.Config
{
	[CustomEditor(typeof(ConfigCollection<>), true)]
	public class ConfigCollectionEditor : Editor
	{
		private IEnumerable<Type> types;

		private SerializedProperty property;
		private IList collectionList;
		private bool isExpanded;
		private IConfigCollection collection;
		private CreateSOMenu creationMenu;
		private SearchField searchField;

		private List<int> indexesToDraw;

		private string search;

		private Dictionary<Object, Editor> cachedEditors = new Dictionary<object, Editor>();

		private void OnEnable()
		{
			collection = (IConfigCollection) serializedObject.targetObject;
			collectionList = collection.List;

			property = serializedObject.FindProperty("configs");

			var argument = serializedObject.targetObject.GetType().BaseType.GetGenericArguments()[0];

			creationMenu = new CreateSOMenu(argument, OnTypeChoose);

			searchField = new SearchField();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			GUILayout.Space(10);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add"))
			{
				creationMenu.Show();
			}

			GUILayout.Space(10);

			if (GUILayout.Button("Toggle All"))
			{
				isExpanded = !isExpanded;
				for (int i = 0; i < property.arraySize; i++)
				{
					property.GetArrayElementAtIndex(i).isExpanded = isExpanded;
				}
			}

			GUILayout.Space(10);

			var oldSearch = search;
			search = searchField.OnGUI(search);

			GUILayout.EndHorizontal();

			GUILayout.BeginVertical();

			if (!property.isArray)
			{
				GUILayout.EndVertical();
				return;
			}

			for (int i = 0; i < property.arraySize; i++)
			{
				var prop = property.GetArrayElementAtIndex(i);
				if (!string.IsNullOrEmpty(search) && prop.objectReferenceValue != null &&
				    !prop.objectReferenceValue.name.Contains(search))
					continue;
				EditorGUILayout.Space(20);
				DrawSO(property.GetArrayElementAtIndex(i), i);
			}

			if (GUILayout.Button("Cleanup"))
			{
				collection.Clean();
			}

			GUILayout.EndVertical();
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawSO(SerializedProperty obj, int i)
		{
			EditorGUILayout.BeginVertical("helpBox");

			GUILayout.Space(10);
			if (obj.objectReferenceValue == null)
			{
				collectionList.RemoveAt(i);
				EditorGUILayout.EndVertical();
				return;
			}

			EditorGUILayout.BeginHorizontal();

			var item = obj.objectReferenceValue;

			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("X", "miniButton", GUILayout.Width(40)))
			{
				collectionList.RemoveAt(i);
				AssetDatabase.RemoveObjectFromAsset(item);
				DestroyImmediate(item);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				return;
			}

			GUI.backgroundColor = Color.white;
			if (i != 0 && GUILayout.Button("▲", "miniButton", GUILayout.Width(40)))
			{
				collectionList[i] = collectionList[i - 1];
				collectionList[i - 1] = item;
			}

			if (i < collectionList.Count - 1 && GUILayout.Button("▼", "miniButton", GUILayout.Width(40)))
			{
				collectionList[i] = collectionList[i + 1];
				collectionList[i + 1] = item;
			}

			item.name = EditorGUILayout.TextField("Name: ", item.name);

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			obj.isExpanded = GUILayout.Button("toggle") ? !obj.isExpanded : obj.isExpanded;
			GUI.enabled = false;
			EditorGUILayout.PropertyField(obj);
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
			if (obj.isExpanded && obj.objectReferenceValue != null)
			{
				if (!cachedEditors.ContainsKey(obj.objectReferenceValue))
					cachedEditors.Add(obj.objectReferenceValue, null);
				Editor editor = cachedEditors[obj.objectReferenceValue];
				CreateCachedEditor(obj.objectReferenceValue, null, ref editor);
				cachedEditors[obj.objectReferenceValue] = editor;
				var box = EditorGUILayout.BeginVertical("frameBox");
				editor.OnInspectorGUI();
				editor.serializedObject.ApplyModifiedProperties();
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.EndVertical();

		}

		private void OnTypeChoose(object userdata)
		{
			var asset = ScriptableObject.CreateInstance(userdata as Type);
			asset.hideFlags = HideFlags.None;
			AssetDatabase.AddObjectToAsset(asset, serializedObject.targetObject);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));

			collection.AddAsset(asset);
			AssetDatabase.Refresh();
		}
	}
}
