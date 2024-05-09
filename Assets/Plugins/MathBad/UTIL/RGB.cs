//  ___   ___  ___                                                                                    
// | _ \ / __|| _ )                                                                                   
// |   /| (_ || _ \                                                                                   
// |_|_\ \___||___/                                                                                   
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using UnityEngine;
#endregion

namespace MathBad
{
public struct ColorKey
{
  public Color color;
  public float key;

  public ColorKey(Color color, float key)
  {
    this.color = color;
    this.key = key;
  }
}

public static class RGBExt
{
  public static float GetValue(this Color color,bool hdr = false)
  {
    Color.RGBToHSV(color, out float h, out float s, out float v);
    return v;
  }
  
  public static Color32 WithSaturation(this Color32 color, float saturation, bool hdr = false)
  {
    Color.RGBToHSV((Color)color, out float h, out float s, out float v);
    s = saturation;
    return (Color32)Color.HSVToRGB(h, s, v, hdr);
  }
  public static Color WithSaturation(this Color color, float saturation, bool hdr = false)
  {
    Color.RGBToHSV(color, out float h, out float s, out float v);
    s = saturation;
    return Color.HSVToRGB(h, s, v, hdr);
  }
  public static Color32 WithValue(this Color32 color, float value, bool hdr = false)
  {
    Color.RGBToHSV((Color)color, out float h, out float s, out float v);
    v = value;
    return (Color32)Color.HSVToRGB(h, s, v, hdr);
  }
  public static Color WithValue(this Color color, float value, bool hdr = false)
  {
    Color.RGBToHSV(color, out float h, out float s, out float v);
    v = value;
    return Color.HSVToRGB(h, s, v, hdr);
  }
}
public class RGB32
{
  /// <summary> (255, 255, 255) </summary>
  public static Color32 clear => new Color32(0, 0, 0, 0);

  /// <summary> (255, 255, 255) </summary>
  public static Color32 white => new Color32(255, 255, 255, 255);
  /// <summary> (230, 230, 230) </summary>
  public static Color32 nearWhite => new Color32(230, 230, 230, 255);
  /// <summary> (191, 191, 191) </summary>
  public static Color32 lightGrey => new Color32(191, 191, 191, 255);
  /// <summary> (128, 128, 128) </summary>
  public static Color32 grey => new Color32(128, 128, 128, 255);
  /// <summary> (64, 64, 64) </summary>
  public static Color32 darkGrey => new Color32(64, 64, 64, 255);
  /// <summary> (26, 26, 26) </summary>
  public static Color32 nearBlack => new Color32(26, 26, 26, 255);
  /// <summary> (0, 0, 0) </summary>
  public static Color32 black => new Color32(0, 0, 0, 255);

  /// <summary> (255, 0, 0) </summary>
  public static Color32 red => new Color32(255, 0, 0, 255);
  /// <summary> (255, 153, 0) </summary>
  public static Color32 orange => new Color32(255, 153, 0, 255);
  /// <summary> (255, 255, 0) </summary>
  public static Color32 yellow => new Color32(255, 255, 0, 255);
  /// <summary> (0, 255, 255) </summary>
  public static Color32 green => new Color32(0, 255, 0, 255);
  /// <summary> (0, 255, 255) </summary>
  public static Color32 cyan => new Color32(0, 255, 255, 255);
  /// <summary> (0, 0, 255) </summary>
  public static Color32 blue => new Color32(0, 0, 255, 255);
  /// <summary> (255, 0, 0) </summary>
  public static Color32 magenta => new Color32(255, 0, 255, 255);

  // Hue / Saturation / Value
  //----------------------------------------------------------------------------------------------------
  public static Color32 Hue(float h, bool hdr = false) => (Color32)Color.HSVToRGB(h, 1f, 1f, hdr);
  public static Color32 Value(float v) => new Color32((byte)(v * 255), (byte)(v * 255), (byte)(v * 255), 255);
}

public class RGB
{
  /// <summary> (1.0f, 1.0f, 1.0f) </summary>
  public static Color clear => new Color(0.0f, 0.0f, 0.0f, 0.0f);

  /// <summary> (1.0f, 1.0f, 1.0f) </summary>
  public static Color white => new Color(1.0f, 1.0f, 1.0f);
  /// <summary> (0.9f, 0.9f, 0.9f) </summary>
  public static Color nearWhite => new Color(0.9f, 0.9f, 0.9f);
  /// <summary> (0.75f, 0.75f, 0.75f) </summary>
  public static Color lightGrey => new Color(0.75f, 0.75f, 0.75f);
  /// <summary> (0.5f, 0.5f, 0.5f) </summary>
  public static Color grey => new Color(0.5f, 0.5f, 0.5f);
  /// <summary> (0.25f, 0.25f, 0.25f) </summary>
  public static Color darkGrey => new Color(0.25f, 0.25f, 0.25f);
  /// <summary> (0.1f, 0.1f, 0.1f) </summary>
  public static Color nearBlack => new Color(0.1f, 0.1f, 0.1f);
  /// <summary> (0.0f, 0.0f, 0.0f) </summary>
  public static Color black => new Color(0.0f, 0.0f, 0.0f);

  /// <summary> (1.0f, 0.0f, 0.0f) </summary>
  public static Color red => new Color(1.0f, 0.0f, 0.0f);
  /// <summary> (1.0f, 0.65f, 0.0f) </summary>
  public static Color orange => new Color(1.0f, 0.65f, 0.0f);
  /// <summary> (1.0f, 1.0f, 0.0f) </summary>
  public static Color yellow => new Color(1.0f, 1.0f, 0.0f);
  /// <summary> (0.0f, 1.0f, 1.0f) </summary>
  public static Color green => new Color(0.0f, 1.0f, 0.0f);
  /// <summary> (0.0f, 1.0f, 1.0f) </summary>
  public static Color cyan => new Color(0.0f, 1.0f, 1.0f);
  /// <summary> (0.0f, 0.0f, 1.0f) </summary>
  public static Color blue => new Color(0.0f, 0.0f, 1.0f);
  /// <summary> (1.0f, 0.0f, 0.0f) </summary>
  public static Color magenta => new Color(1.0f, 0.0f, 1.0f);

  // Hue / Saturation / Value
  //----------------------------------------------------------------------------------------------------
  public static Color Hue(float h, bool hdr = false) => Color.HSVToRGB(h, 1f, 1f, hdr);
  public static Color HueLerp(int index, int length) => Hue((float)index / (length - 1));
  public static Color Value(float v) => new Color(v, v, v, 1.0f);

  // Random
  //----------------------------------------------------------------------------------------------------
  public static Color NextHue() => Hue(RNG.Float());
  public static Color Condition(bool flag) => flag ? green : red;
}

// Gradient
//----------------------------------------------------------------------------------------------------
public static class GRADIENT
{
  public static readonly Gradient blackWhite = Create(new[] {new ColorKey(RGB.black, 0f), new ColorKey(RGB.white, 1f)});
  public static readonly Gradient weightGradient
    = Create(new[]
             {
               new ColorKey(RGB.black, 0.0f), // 0
               new ColorKey(RGB.blue, 0.02f),
               new ColorKey(RGB.cyan, 0.25f),
               new ColorKey(RGB.green, 0.5f),
               new ColorKey(RGB.yellow, 0.75f),
               new ColorKey(RGB.red, 0.98f),
               new ColorKey(RGB.white, 1.0f) // 1
             });
  public static readonly Gradient heatGradient
    = Create(new[]
             {
               new ColorKey(RGB.black, 0.0f), // 0
               new ColorKey(RGB.blue, 0.02f),
               new ColorKey(RGB.magenta, 0.25f),
               new ColorKey(RGB.red, 0.5f),
               new ColorKey(RGB.orange, 0.75f),
               new ColorKey(RGB.yellow, 0.98f),
               new ColorKey(RGB.white, 1.0f) // 1
             });
  public static readonly Gradient dangerGradient
    = Create(new[]
             {
               new ColorKey(RGB.blue, 0.0f), // 0
               new ColorKey(RGB.cyan, 0.02f),
               new ColorKey(RGB.green, 0.25f),
               new ColorKey(RGB.yellow, 0.5f),
               new ColorKey(RGB.orange, 0.75f),
               new ColorKey(RGB.red, 0.98f),
               new ColorKey(RGB.white, 1.0f) // 1
             });
  public static readonly Gradient greyGradient
    = Create(new[]
             {
               new ColorKey(RGB.darkGrey, 0.0f), // 0
               new ColorKey(RGB.grey, 0.35f),
               new ColorKey(RGB.lightGrey, 0.65f),
               new ColorKey(RGB.nearWhite, 1.0f) // 1
             });

  // Create
  //----------------------------------------------------------------------------------------------------
  public static Gradient Create(ColorKey[] keys)
  {
    Gradient gradient = new Gradient();
    gradient.mode = GradientMode.Blend;

    int keyCount = keys.Length;
    GradientColorKey[] colorKeys = new GradientColorKey[keyCount];
    GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
    alphaKeys[0] = new GradientAlphaKey(1f, 0f);
    alphaKeys[1] = new GradientAlphaKey(1f, 1f);

    for(int i = 0; i < keyCount; i++)
      colorKeys[i] = new GradientColorKey(keys[i].color, keys[i].key);

    gradient.SetKeys(colorKeys, alphaKeys);
    return gradient;
  }

  public static Gradient Create(params Color[] keys)
  {
    if(keys.IsNullOrEmpty())
    {
      Debug.LogError("GradientUtil.Create needs a valid Color[] key.");
    }
    Gradient gradient = new Gradient();
    gradient.mode = GradientMode.Blend;

    int keyCount = keys.Length;
    GradientColorKey[] colorKeys = new GradientColorKey[keyCount];
    GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
    alphaKeys[0] = new GradientAlphaKey(1f, 0f);
    alphaKeys[1] = new GradientAlphaKey(1f, 1f);

    for(int i = 0; i < keyCount; i++)
      colorKeys[i] = new GradientColorKey(keys[i], (float)i / (keyCount - 1));

    gradient.SetKeys(colorKeys, alphaKeys);
    return gradient;
  }
}
}
