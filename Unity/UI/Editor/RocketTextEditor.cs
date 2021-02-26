using System.Reflection;
using RocketWorks;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(RocketText), true)]
    [CanEditMultipleObjects]
    public class RocketTextEditor : TMP_EditorPanelUI
    {
        SerializedProperty m_Text;
        SerializedProperty m_FontData;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Text = serializedObject.FindProperty("m_text");
            m_FontData = serializedObject.FindProperty("rocketFont");
        }

        public override void OnInspectorGUI()
        {
            // Make sure Multi selection only includes TMP Text objects.
            if (IsMixSelectionTypes()) return;

            serializedObject.Update();

            var rocketText = target as RocketText;
            if (!rocketText.HasTextController)
                DrawTextInput();

            EditorGUILayout.PropertyField(m_FontData);

            DrawMainSettings();

            DrawExtraSettings();

            EditorGUILayout.Space();

            if (serializedObject.ApplyModifiedProperties() || m_HavePropertiesChanged)
            {
                m_TextComponent.havePropertiesChanged = true;
                m_HavePropertiesChanged = false;
                EditorUtility.SetDirty(target);
            }
        }
    }
}