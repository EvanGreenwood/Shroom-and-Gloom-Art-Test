#region
using MathBad;
using UnityEditor;
using UnityEngine;
#endregion

namespace MathBad_Editor
{
public static class EditorGUIExt
{
  public static void Draw(this Rect rect, Color color) => EditorGUI.DrawRect(rect, color);
  public static void Draw(this Rect rect,
                          Texture2D texture,
                          Color color,
                          bool alpha = false,
                          ScaleMode scaleMode = ScaleMode.StretchToFill)
    => GUI.DrawTexture(rect, texture, scaleMode, alpha, 0f, color, Vector4.zero, Vector4.zero);

  public static void DrawBorder(this Rect rect, float thickness, Color color)
  {
    Rect.MinMaxRect(rect.min.x - thickness.Half(), rect.position.y - thickness.Half(), rect.max.x + thickness.Half(), rect.position.y + thickness.Half()).Draw(color);
    Rect.MinMaxRect(rect.min.x - thickness.Half(), rect.position.y + rect.height - thickness.Half(), rect.max.x + thickness.Half(), rect.position.y + rect.height + thickness.Half()).Draw(color);
    Rect.MinMaxRect(rect.position.x + rect.width - thickness.Half(), rect.min.y - thickness.Half(), rect.position.x + rect.width + thickness.Half(), rect.max.y + thickness.Half()).Draw(color);
    Rect.MinMaxRect(rect.position.x - thickness.Half(), rect.min.y - thickness.Half(), rect.position.x + thickness.Half(), rect.max.y + thickness.Half()).Draw(color);
  }
}
}
