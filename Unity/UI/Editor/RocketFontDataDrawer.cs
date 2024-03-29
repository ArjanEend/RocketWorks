﻿using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomPropertyDrawer(typeof(RocketFontData), true)]
    public class RocketFontDataDrawer : PropertyDrawer
    {
        static private class Styles
        {
            public static GUIStyle alignmentButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
            public static GUIStyle alignmentButtonMid = new GUIStyle(EditorStyles.miniButtonMid);
            public static GUIStyle alignmentButtonRight = new GUIStyle(EditorStyles.miniButtonRight);

            public static GUIContent m_EncodingContent;

            public static GUIContent m_LeftAlignText;
            public static GUIContent m_CenterAlignText;
            public static GUIContent m_RightAlignText;
            public static GUIContent m_TopAlignText;
            public static GUIContent m_MiddleAlignText;
            public static GUIContent m_BottomAlignText;

            public static GUIContent m_LeftAlignTextActive;
            public static GUIContent m_CenterAlignTextActive;
            public static GUIContent m_RightAlignTextActive;
            public static GUIContent m_TopAlignTextActive;
            public static GUIContent m_MiddleAlignTextActive;
            public static GUIContent m_BottomAlignTextActive;

            static Styles()
            {
                m_EncodingContent = new GUIContent("Rich Text", "Use emoticons and colors");

                // Horizontal Aligment Icons
                m_LeftAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_left", "Left Align");
                m_CenterAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_center", "Center Align");
                m_RightAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_right", "Right Align");
                m_LeftAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_left_active", "Left Align");
                m_CenterAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_center_active", "Center Align");
                m_RightAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_right_active", "Right Align");

                // Vertical Aligment Icons
                m_TopAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_top", "Top Align");
                m_MiddleAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_center", "Middle Align");
                m_BottomAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_bottom", "Bottom Align");
                m_TopAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_top_active", "Top Align");
                m_MiddleAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_center_active", "Middle Align");
                m_BottomAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_bottom_active", "Bottom Align");

                FixAlignmentButtonStyles(alignmentButtonLeft, alignmentButtonMid, alignmentButtonRight);
            }

            static void FixAlignmentButtonStyles(params GUIStyle[] styles)
            {
                foreach (GUIStyle style in styles)
                {
                    style.padding.left = 2;
                    style.padding.right = 2;
                }
            }
        }

        private enum VerticalTextAligment
        {
            Top,
            Middle,
            Bottom
        }

        private enum HorizontalTextAligment
        {
            Left,
            Center,
            Right
        }

        private const int kAlignmentButtonWidth = 20;

        static int s_TextAlignmentHash = "DoTextAligmentControl".GetHashCode();

        private SerializedProperty m_styleAsset;
        private SerializedProperty m_SupportEncoding;
        private SerializedProperty m_ResizeTextForBestFit;
        private SerializedProperty m_ResizeTextMinSize;
        private SerializedProperty m_ResizeTextMaxSize;
        private SerializedProperty m_HorizontalOverflow;
        private SerializedProperty m_VerticalOverflow;
        private SerializedProperty m_Alignment;

        private float m_FontFieldfHeight = 0f;
        private float m_FontStyleHeight = 0f;
        private float m_FontSizeHeight = 0f;
        private float m_LineSpacingHeight = 0f;
        private float m_EncodingHeight = 0f;
        private float m_ResizeTextForBestFitHeight = 0f;
        private float m_ResizeTextMinSizeHeight = 0f;
        private float m_ResizeTextMaxSizeHeight = 0f;
        private float m_HorizontalOverflowHeight = 0f;
        private float m_VerticalOverflowHeight = 0f;

        protected void Init(SerializedProperty property)
        {
            m_styleAsset = property.FindPropertyRelative("fontStyle");
            m_SupportEncoding = property.FindPropertyRelative("m_RichText");
            m_ResizeTextForBestFit = property.FindPropertyRelative("m_BestFit");
            m_ResizeTextMinSize = property.FindPropertyRelative("m_MinSize");
            m_ResizeTextMaxSize = property.FindPropertyRelative("m_MaxSize");
            m_HorizontalOverflow = property.FindPropertyRelative("m_HorizontalOverflow");
            m_VerticalOverflow = property.FindPropertyRelative("m_VerticalOverflow");
            m_Alignment = property.FindPropertyRelative("m_Alignment");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Init(property);
            m_EncodingHeight = EditorGUI.GetPropertyHeight(m_SupportEncoding);
            m_ResizeTextForBestFitHeight = EditorGUI.GetPropertyHeight(m_ResizeTextForBestFit);
            m_ResizeTextMinSizeHeight = EditorGUI.GetPropertyHeight(m_ResizeTextMinSize);
            m_ResizeTextMaxSizeHeight = EditorGUI.GetPropertyHeight(m_ResizeTextMaxSize);
            m_HorizontalOverflowHeight = EditorGUI.GetPropertyHeight(m_HorizontalOverflow);
            m_VerticalOverflowHeight = EditorGUI.GetPropertyHeight(m_VerticalOverflow);

            var height = m_FontFieldfHeight
                       + m_FontStyleHeight
                       + m_FontSizeHeight
                       + m_LineSpacingHeight
                       + m_EncodingHeight
                       + m_ResizeTextForBestFitHeight
                       + m_HorizontalOverflowHeight
                       + m_VerticalOverflowHeight
                       + EditorGUIUtility.singleLineHeight * 4
                       + EditorGUIUtility.standardVerticalSpacing * 10;

            if (m_ResizeTextForBestFit.boolValue)
            {
                height += m_ResizeTextMinSizeHeight
                    + m_ResizeTextMaxSizeHeight
                    + EditorGUIUtility.standardVerticalSpacing * 2;
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);

            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(rect, m_styleAsset);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.LabelField(rect, "Character", EditorStyles.boldLabel);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            ++EditorGUI.indentLevel;
            {
                rect.height = m_FontFieldfHeight;
                EditorGUI.BeginChangeCheck();

                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = m_FontStyleHeight;
                EditorGUI.EndDisabledGroup();

                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = m_EncodingHeight;
                EditorGUI.PropertyField(rect, m_SupportEncoding, Styles.m_EncodingContent);
            }
            --EditorGUI.indentLevel;

            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, "Paragraph", EditorStyles.boldLabel);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            ++EditorGUI.indentLevel;
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                DoTextAligmentControl(rect, m_Alignment);

                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = m_HorizontalOverflowHeight;
                EditorGUI.PropertyField(rect, m_HorizontalOverflow);

                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = m_VerticalOverflowHeight;
                EditorGUI.PropertyField(rect, m_VerticalOverflow);

                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = m_ResizeTextMaxSizeHeight;
                EditorGUI.PropertyField(rect, m_ResizeTextForBestFit);

                if (m_ResizeTextForBestFit.boolValue)
                {
                    EditorGUILayout.EndFadeGroup();
                    rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                    rect.height = m_ResizeTextMinSizeHeight;
                    EditorGUI.PropertyField(rect, m_ResizeTextMinSize);

                    rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                    rect.height = m_ResizeTextMaxSizeHeight;
                    EditorGUI.PropertyField(rect, m_ResizeTextMaxSize);
                }
            }
            --EditorGUI.indentLevel;
        }

        private void DoTextAligmentControl(Rect position, SerializedProperty alignment)
        {
            GUIContent alingmentContent = new GUIContent("Alignment");

            int id = EditorGUIUtility.GetControlID(s_TextAlignmentHash, EditorGUIUtility.native, position);

            EditorGUIUtility.SetIconSize(new Vector2(15, 15));
            EditorGUI.BeginProperty(position, alingmentContent, alignment);
            {
                Rect controlArea = EditorGUI.PrefixLabel(position, id, alingmentContent);

                float width = kAlignmentButtonWidth * 3;
                float spacing = Mathf.Clamp(controlArea.width - width * 2, 2, 10);

                Rect horizontalAligment = new Rect(controlArea.x, controlArea.y, width, controlArea.height);
                Rect verticalAligment = new Rect(horizontalAligment.xMax + spacing, controlArea.y, width, controlArea.height);

                DoHorizontalAligmentControl(horizontalAligment, alignment);
                DoVerticalAligmentControl(verticalAligment, alignment);
            }
            EditorGUI.EndProperty();
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        private static void DoHorizontalAligmentControl(Rect position, SerializedProperty alignment)
        {
            TextAnchor ta = (TextAnchor)alignment.intValue;
            HorizontalTextAligment horizontalAlignment = GetHorizontalAlgiment(ta);

            bool leftAlign = (horizontalAlignment == HorizontalTextAligment.Left);
            bool centerAlign = (horizontalAlignment == HorizontalTextAligment.Center);
            bool rightAlign = (horizontalAlignment == HorizontalTextAligment.Right);

            if (alignment.hasMultipleDifferentValues)
            {
                foreach (var obj in alignment.serializedObject.targetObjects)
                {
                    Text text = obj as Text;
                    horizontalAlignment = GetHorizontalAlgiment(text.alignment);
                    leftAlign = leftAlign || (horizontalAlignment == HorizontalTextAligment.Left);
                    centerAlign = centerAlign || (horizontalAlignment == HorizontalTextAligment.Center);
                    rightAlign = rightAlign || (horizontalAlignment == HorizontalTextAligment.Right);
                }
            }

            position.width = kAlignmentButtonWidth;

            EditorGUI.BeginChangeCheck();
            EditorToggle(position, leftAlign, leftAlign ? Styles.m_LeftAlignTextActive : Styles.m_LeftAlignText, Styles.alignmentButtonLeft);
            if (EditorGUI.EndChangeCheck())
            {
                SetHorizontalAlignment(alignment, HorizontalTextAligment.Left);
            }

            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, centerAlign, centerAlign ? Styles.m_CenterAlignTextActive : Styles.m_CenterAlignText, Styles.alignmentButtonMid);
            if (EditorGUI.EndChangeCheck())
            {
                SetHorizontalAlignment(alignment, HorizontalTextAligment.Center);
            }

            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, rightAlign, rightAlign ? Styles.m_RightAlignTextActive : Styles.m_RightAlignText, Styles.alignmentButtonRight);
            if (EditorGUI.EndChangeCheck())
            {
                SetHorizontalAlignment(alignment, HorizontalTextAligment.Right);
            }
        }

        private static void DoVerticalAligmentControl(Rect position, SerializedProperty alignment)
        {
            TextAnchor ta = (TextAnchor)alignment.intValue;
            VerticalTextAligment verticalTextAligment = GetVerticalAlgiment(ta);

            bool topAlign = (verticalTextAligment == VerticalTextAligment.Top);
            bool middleAlign = (verticalTextAligment == VerticalTextAligment.Middle);
            bool bottomAlign = (verticalTextAligment == VerticalTextAligment.Bottom);

            if (alignment.hasMultipleDifferentValues)
            {
                foreach (var obj in alignment.serializedObject.targetObjects)
                {
                    Text text = obj as Text;
                    TextAnchor textAlignment = text.alignment;
                    verticalTextAligment = GetVerticalAlgiment(textAlignment);
                    topAlign = topAlign || (verticalTextAligment == VerticalTextAligment.Top);
                    middleAlign = middleAlign || (verticalTextAligment == VerticalTextAligment.Middle);
                    bottomAlign = bottomAlign || (verticalTextAligment == VerticalTextAligment.Bottom);
                }
            }


            position.width = kAlignmentButtonWidth;

            // position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, topAlign, topAlign ? Styles.m_TopAlignTextActive : Styles.m_TopAlignText, Styles.alignmentButtonLeft);
            if (EditorGUI.EndChangeCheck())
            {
                SetVerticalAlignment(alignment, VerticalTextAligment.Top);
            }

            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, middleAlign, middleAlign ? Styles.m_MiddleAlignTextActive : Styles.m_MiddleAlignText, Styles.alignmentButtonMid);
            if (EditorGUI.EndChangeCheck())
            {
                SetVerticalAlignment(alignment, VerticalTextAligment.Middle);
            }

            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, bottomAlign, bottomAlign ? Styles.m_BottomAlignTextActive : Styles.m_BottomAlignText, Styles.alignmentButtonRight);
            if (EditorGUI.EndChangeCheck())
            {
                SetVerticalAlignment(alignment, VerticalTextAligment.Bottom);
            }
        }

        private static bool EditorToggle(Rect position, bool value, GUIContent content, GUIStyle style)
        {
            int hashCode = "AlignToggle".GetHashCode();
            int id = EditorGUIUtility.GetControlID(hashCode, EditorGUIUtility.native, position);
            Event evt = Event.current;

            // Toggle selected toggle on space or return key
            if (EditorGUIUtility.keyboardControl == id && evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Space || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter))
            {
                value = !value;
                evt.Use();
                GUI.changed = true;
            }

            if (evt.type == EventType.KeyDown && Event.current.button == 0 && position.Contains(Event.current.mousePosition))
            {
                GUIUtility.keyboardControl = id;
                EditorGUIUtility.editingTextField = false;
                HandleUtility.Repaint();
            }

            bool returnValue = GUI.Toggle(position, id, value, content, style);

            return returnValue;
        }

        private static HorizontalTextAligment GetHorizontalAlgiment(TextAnchor ta)
        {
            switch (ta)
            {
                case TextAnchor.MiddleCenter:
                case TextAnchor.UpperCenter:
                case TextAnchor.LowerCenter:
                    return HorizontalTextAligment.Center;

                case TextAnchor.UpperRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.LowerRight:
                    return HorizontalTextAligment.Right;

                case TextAnchor.UpperLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.LowerLeft:
                    return HorizontalTextAligment.Left;
            }

            return HorizontalTextAligment.Left;
        }

        private static VerticalTextAligment GetVerticalAlgiment(TextAnchor ta)
        {
            switch (ta)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperCenter:
                case TextAnchor.UpperRight:
                    return VerticalTextAligment.Top;

                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    return VerticalTextAligment.Middle;

                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    return VerticalTextAligment.Bottom;
            }

            return VerticalTextAligment.Top;
        }

        // We can't go through serialzied properties here since we're showing two controls for a single SerializzedProperty.
        private static void SetHorizontalAlignment(SerializedProperty alignment, HorizontalTextAligment horizontalAlignment)
        {
            foreach (var obj in alignment.serializedObject.targetObjects)
            {
                Text text = obj as Text;
                VerticalTextAligment currentVerticalAligment = GetVerticalAlgiment(text.alignment);
                Undo.RecordObject(text, "Horizontal Alignment");
                text.alignment = GetAnchor(currentVerticalAligment, horizontalAlignment);
                EditorUtility.SetDirty(obj);
            }
        }

        private static void SetVerticalAlignment(SerializedProperty alignment, VerticalTextAligment verticalAlignment)
        {
            foreach (var obj in alignment.serializedObject.targetObjects)
            {
                Text text = obj as Text;
                HorizontalTextAligment currentHorizontalAligment = GetHorizontalAlgiment(text.alignment);
                Undo.RecordObject(text, "Vertical Alignment");
                text.alignment = GetAnchor(verticalAlignment, currentHorizontalAligment);
                EditorUtility.SetDirty(obj);
            }
        }

        private static TextAnchor GetAnchor(VerticalTextAligment verticalTextAligment, HorizontalTextAligment horizontalTextAligment)
        {
            TextAnchor ac = TextAnchor.UpperLeft;

            switch (horizontalTextAligment)
            {
                case HorizontalTextAligment.Left:
                    switch (verticalTextAligment)
                    {
                        case VerticalTextAligment.Bottom:
                            ac = TextAnchor.LowerLeft;
                            break;
                        case VerticalTextAligment.Middle:
                            ac = TextAnchor.MiddleLeft;
                            break;
                        default:
                            ac = TextAnchor.UpperLeft;
                            break;
                    }
                    break;
                case HorizontalTextAligment.Center:
                    switch (verticalTextAligment)
                    {
                        case VerticalTextAligment.Bottom:
                            ac = TextAnchor.LowerCenter;
                            break;
                        case VerticalTextAligment.Middle:
                            ac = TextAnchor.MiddleCenter;
                            break;
                        default:
                            ac = TextAnchor.UpperCenter;
                            break;
                    }
                    break;
                default:
                    switch (verticalTextAligment)
                    {
                        case VerticalTextAligment.Bottom:
                            ac = TextAnchor.LowerRight;
                            break;
                        case VerticalTextAligment.Middle:
                            ac = TextAnchor.MiddleRight;
                            break;
                        default:
                            ac = TextAnchor.UpperRight;
                            break;
                    }
                    break;
            }
            return ac;
        }
    }
}