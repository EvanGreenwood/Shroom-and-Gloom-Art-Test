using UnityEngine;
namespace MathBad
{
public class LERP
{
  public static Color Lerp(Color a, Color b, float t, EaseType easeType = EaseType.Linear)
  {
    Color result = Color.Lerp(a, b, EASE.Evaluate(t, easeType));
    return result;
  }
}
}
