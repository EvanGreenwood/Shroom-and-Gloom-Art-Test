using UnityEngine;
namespace Mainframe
{
public class LERP
{
  public static Color Lerp(Color a, Color b, float t, Easing easing = Easing.Linear)
  {
    Color result = Color.Lerp(a, b, EASE.Evaluate(t, easing));
    return result;
  }
}
}
