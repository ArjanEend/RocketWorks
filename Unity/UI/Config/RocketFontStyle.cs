using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RocketWorks/UI/FontStyle", fileName = "New FontStyle")]
public class RocketFontStyle : ScriptableObject
{
    [SerializeField]
    private TMPro.TMP_FontAsset  font;

    public TMPro.TMP_FontAsset Font => font;

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
    private TMPro.FontStyles fontStyle = TMPro.FontStyles.Normal;

    public TMPro.FontStyles FontStyle => fontStyle;

    [SerializeField]
    private float lineSpacing = 1f;
    public float LineSpacing => lineSpacing;

}
