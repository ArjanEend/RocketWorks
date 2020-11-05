using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(RocketText), true)]
    [CanEditMultipleObjects]
    public class RocketTextEditor : TextEditor
    {
        SerializedProperty m_Text;
        SerializedProperty m_FontData;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("rocketFontData");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var rocketText = target as RocketText;
            if (!rocketText.HasTextController)
                EditorGUILayout.PropertyField(m_Text);

            if (m_FontData != null)
                EditorGUILayout.PropertyField(m_FontData);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}