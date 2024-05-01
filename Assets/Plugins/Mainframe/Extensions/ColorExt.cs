#region
using UnityEngine;
#endregion
namespace Mainframe
{
public static class ColorExt
{
  public static Color WithA(this Color c, float a) => new Color(c.r, c.g, c.b, a);
  public static Color32 WithA(this Color32 c, byte a) => new Color(c.r, c.g, c.b, a);

  public static Color Invert(this Color c) => new Color(1f - c.r, 1f - c.g, 1f - c.b, c.a);
  public static Color32 Invert(this Color32 c) => new Color32((byte)(255 - c.r), (byte)(255 - c.g), (byte)(255 - c.b), c.a);
}
}
