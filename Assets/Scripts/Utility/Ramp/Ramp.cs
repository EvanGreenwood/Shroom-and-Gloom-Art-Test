#region Usings
using System.Collections.Generic;
using AssetIcons;
using UnityEngine;
using MathBad;
#endregion

[MenuCreate("RampTexture", 0)]
[CreateAssetMenu(fileName = "Ramp", menuName = "Ramp", order = 0)]
public class Ramp : ScriptableObject
{
    public enum RampUseFlags
    {
        A_Horizontal = 0,
        B_Horizontal = 1,
        AB_TopBottom = 2,
        AB_HorizontalVertical = 3,
    }

    public RampUseFlags useFlags = RampUseFlags.A_Horizontal;

    public Gradient gradientA = new Gradient();
    public Gradient gradientB = new Gradient();

    public EaseType easeType = EaseType.Linear;

    public bool sRGB = true;
    public bool alphaIsTransparency = false;
    public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
    public FilterMode filterMode = FilterMode.Bilinear;

    //--------------------------------------------------
    [HideInInspector]
    public bool alwaysUpdate;

    [SerializeField, HideInInspector]
    Texture2D _texture;
    [AssetIcon]
    public Sprite icon;

    [SerializeField, HideInInspector]
    List<Object> _children = new List<Object>();

    public Texture2D texture
    {
        get => _texture;
        set => _texture = value;
    }

    public List<Object> children
    {
        get => _children;
        set => _children = value;
    }
}
