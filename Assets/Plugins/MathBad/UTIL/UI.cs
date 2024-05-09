#region
using UnityEngine;
#endregion

namespace MathBad
{
public static class UI
{
  static UI()
  {
    unsciiThin = Resources.Load<Font>("Fonts/unscii-8-thin");
    debugStyle = new GUIStyle();
    debugStyle.font = unsciiThin;
    debugStyle.normal.textColor = RGB.white;
    debugStyle.normal.background = Texture2D.blackTexture;
    debugStyle.fontSize = 14;
  }
  public static readonly Font unsciiThin;
  public static readonly GUIStyle debugStyle;
}
}
