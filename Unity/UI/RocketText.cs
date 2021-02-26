using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using TMPro;
using UnityEditor;
#endif

namespace RocketWorks
{
    public class RocketText : TextMeshProUGUI
    {
        [SerializeField, NonEditable]
        private bool hasTextController;
        public bool HasTextController => hasTextController;

        [SerializeField] private RocketFontStyle rocketFont;

#if UNITY_EDITOR
        override protected void OnValidate()
        {
            base.OnValidate();

            if (fontStyle == null)
                return;
            
            UpdateTextValues();
        }
        
        [MenuItem("CONTEXT/TextMeshProUGUI/Replace RocketText")]
        public static void ReplaceTMPPro(MenuCommand command)
        {
            
            TextMeshProUGUI tmpPro = (TextMeshProUGUI) command.context;
            var go = tmpPro.gameObject;
            DestroyImmediate(tmpPro);
            go.AddComponent<RocketText>();
        }
#endif

        private void UpdateTextValues()
        {
            if (Application.isPlaying)
                return;
            if (GetComponent<ITextController>() is ITextController controller)
            {
                hasTextController = true;
                text = controller.PreviewText;
            }
            else
            {
                hasTextController = false;
            }

            font = rocketFont.Font;
            color = rocketFont.Color;
            fontSize = rocketFont.FontSize;
            base.fontStyle = rocketFont.FontStyle;
            lineSpacing = rocketFont.LineSpacing;
            
        }

        protected override void UpdateGeometry()
        {
            UpdateTextValues();
            base.UpdateGeometry();
        }

        public override void Rebuild(CanvasUpdate update)
        {
            UpdateTextValues();
            base.Rebuild(update);
        }
    }
}
