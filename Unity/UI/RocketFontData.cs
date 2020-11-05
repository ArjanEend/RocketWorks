using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RocketFontData : FontData
{
    [SerializeField]
    private RocketFontStyle fontStyle;

    public RocketFontStyle FontStyle => fontStyle;

    public Color Color => fontStyle.Color;
}
