using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RocketWorks/UI/FontStyle", fileName = "New FontStyle")]
public class RocketFontStyle : ScriptableObject
{
    [SerializeField]
    private Font font;

    public Font Font => font;

    [SerializeField]
    private int fontSize;

    public int FontSize => fontSize;

    [SerializeField]
    private Color color;

    public Color Color => color;

    [SerializeField]
    private int outline;

    public int Outline => outline;

    [SerializeField]
    private Color outlineColor;

    public Color OutlineColor => outlineColor;

    [SerializeField]
    private FontStyle fontStyle = FontStyle.Normal;

    public FontStyle FontStyle => fontStyle;

    [SerializeField]
    private float lineSpacing = 1f;
    public float LineSpacing => lineSpacing;

}
