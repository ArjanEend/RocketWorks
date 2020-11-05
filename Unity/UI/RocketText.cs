using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class RocketText : Text
{
    [SerializeField]
    private RocketFontData rocketFontData = new RocketFontData();

    [SerializeField, NonEditable]
    private bool hasTextController;
    public bool HasTextController => hasTextController;

#if UNITY_EDITOR
    override protected void OnValidate()
    {
        base.OnValidate();
        if (rocketFontData == null)
            rocketFontData = new RocketFontData();
        var fontData = GetType().BaseType.GetField("m_FontData",
            BindingFlags.Instance | BindingFlags.NonPublic);

        var value = fontData.GetValue(this);
        if (value.GetType() == typeof(FontData))
        {
            fontData.SetValue(this, rocketFontData);
        }

        UpdateTextValues();
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

        font = rocketFontData.FontStyle.Font;
        color = rocketFontData.Color;
        fontSize = rocketFontData.FontStyle.FontSize;
        fontStyle = rocketFontData.FontStyle.FontStyle;
        lineSpacing = rocketFontData.FontStyle.LineSpacing;
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
