//  ___    _    ___  ___                                                                              
// | __|  /_\  / __|| __|                                                                             
// | _|  / _ \ \__ \| _|                                                                              
// |___|/_/ \_\|___/|___|                                                                             
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using System;
using UnityEngine;
#endregion
namespace Mainframe
{
public enum EaseType
{
  Linear = 0,
  InQuad, OutQuad, InOutQuad,
  InCubic, OutCubic, InOutCubic,
  InQuart, OutQuart, InOutQuart,
  InSine, OutSine, InOutSine,
  InExpo, OutExpo, InOutExpo,
  InCirc, OutCirc, InOutCirc,
  InElastic, OutElastic, InOutElastic,
  InBack, OutBack, InOutBack,
  InBounce, OutBounce, InOutBounce,
}
public static class EASE
{
  public static float Evaluate(float t, EaseType type)
  {
    return type switch
           {
             EaseType.Linear       => Linear(t),
             EaseType.InQuad       => InQuad(t),
             EaseType.OutQuad      => OutQuad(t),
             EaseType.InOutQuad    => InOutQuad(t),
             EaseType.InCubic      => InCubic(t),
             EaseType.OutCubic     => OutCubic(t),
             EaseType.InOutCubic   => InOutCubic(t),
             EaseType.InQuart      => InQuart(t),
             EaseType.OutQuart     => OutQuart(t),
             EaseType.InOutQuart   => InOutQuart(t),
             EaseType.InSine       => InSine(t),
             EaseType.OutSine      => OutSine(t),
             EaseType.InOutSine    => InOutSine(t),
             EaseType.InExpo       => InExpo(t),
             EaseType.OutExpo      => OutExpo(t),
             EaseType.InOutExpo    => InOutExpo(t),
             EaseType.InCirc       => InCirc(t),
             EaseType.OutCirc      => OutCirc(t),
             EaseType.InOutCirc    => InOutCirc(t),
             EaseType.InElastic    => InElastic(t),
             EaseType.OutElastic   => OutElastic(t),
             EaseType.InOutElastic => InOutElastic(t),
             EaseType.InBack       => InBack(t),
             EaseType.OutBack      => OutBack(t),
             EaseType.InOutBack    => InOutBack(t),
             EaseType.InBounce     => InBounce(t),
             EaseType.OutBounce    => OutBounce(t),
             EaseType.InOutBounce  => InOutBounce(t),
             _                     => Linear(t)
           };
  }

  // Linear
  //----------------------------------------------------------------------------------------------------
  public static float Linear(float t) => t;

  // Quadratic
  //----------------------------------------------------------------------------------------------------
  public static float InQuad(float t) => t * t;
  public static float OutQuad(float t) => 1 - InQuad(1 - t);
  public static float InOutQuad(float t)
  {
    if(t < 0.5f) return InQuad(t * 2f) / 2f;
    return 1f - InQuad((1f - t) * 2f) / 2f;
  }

  // Cubic
  //----------------------------------------------------------------------------------------------------
  public static float InCubic(float t) => t * t * t;
  public static float OutCubic(float t) => 1 - InCubic(1 - t);
  public static float InOutCubic(float t)
  {
    if(t < 0.5f) return InCubic(t * 2f) / 2f;
    return 1f - InCubic((1f - t) * 2f) / 2f;
  }

  // Quartic
  //----------------------------------------------------------------------------------------------------
  public static float InQuart(float t) => t * t * t * t;
  public static float OutQuart(float t) => 1 - InQuart(1 - t);
  public static float InOutQuart(float t)
  {
    if(t < 0.5f) return InQuart(t * 2f) / 2f;
    return 1f - InQuart((1f - t) * 2f) / 2f;
  }

  // Quintic
  //----------------------------------------------------------------------------------------------------
  public static float InQuint(float t) => t * t * t * t * t;
  public static float OutQuint(float t) => 1 - InQuint(1 - t);
  public static float InOutQuint(float t)
  {
    if(t < 0.5f) return InQuint(t * 2f) / 2f;
    return 1f - InQuint((1f - t) * 2f) / 2f;
  }

  // Sine
  //----------------------------------------------------------------------------------------------------
  public static float InSine(float t) => (float)-Math.Cos(t * Math.PI / 2);
  public static float OutSine(float t) => (float)Math.Sin(t * Math.PI / 2);
  public static float InOutSine(float t) => (float)(Math.Cos(t * Math.PI) - 1) * -0.5f;

  // Exponential
  //----------------------------------------------------------------------------------------------------
  public static float InExpo(float t) => (float)Math.Pow(2, 10 * (t - 1));
  public static float OutExpo(float t) => 1 - InExpo(1 - t);
  public static float InOutExpo(float t)
  {
    if(t < 0.5f) return InExpo(t * 2f) * 0.5f;
    return 1f - InExpo((1f - t) * 2f) * 0.5f;
  }

  // Circular
  //----------------------------------------------------------------------------------------------------
  public static float InCirc(float t) => -((float)Math.Sqrt(1 - t * t) - 1);
  public static float OutCirc(float t) => 1 - InCirc(1 - t);
  public static float InOutCirc(float t)
  {
    if(t < 0.5f) return InCirc(t * 2f) * 0.5f;
    return 1f - InCirc((1f - t) * 2f) * 0.5f;
  }

  // Elastic
  //----------------------------------------------------------------------------------------------------
  public static float InElastic(float t) => 1 - OutElastic(1 - t);
  public static float OutElastic(float t)
  {
    float p = 0.3f;
    return (float)Mathf.Pow(2, -10 * t) * (float)Mathf.Sin((t - p / 4) * (2 * Mathf.PI) / p) + 1;
  }
  public static float InOutElastic(float t)
  {
    if(t < 0.5f) return InElastic(t * 2f) * 0.5f;
    return 1f - InElastic((1f - t) * 2f) * 0.5f;
  }

  // Back
  //----------------------------------------------------------------------------------------------------
  public static float InBack(float t)
  {
    float s = 1.70158f;
    return t * t * ((s + 1f) * t - s);
  }
  public static float OutBack(float t) => 1f - InBack(1f - t);
  public static float InOutBack(float t)
  {
    if(t < 0.5f) return InBack(t * 2f) * 0.5f;
    return 1f - InBack((1f - t) * 2f) * 0.5f;
  }

  // Bounce
  //----------------------------------------------------------------------------------------------------
  public static float InBounce(float t) => 1 - OutBounce(1 - t);
  public static float OutBounce(float t)
  {
    float div = 2.75f;
    float mult = 7.5625f;

    if(t < 1 / div)
    {
      return mult * t * t;
    }
    else if(t < 2 / div)
    {
      t -= 1.5f / div;
      return mult * t * t + 0.75f;
    }
    else if(t < 2.5 / div)
    {
      t -= 2.25f / div;
      return mult * t * t + 0.9375f;
    }
    else
    {
      t -= 2.625f / div;
      return mult * t * t + 0.984375f;
    }
  }
  public static float InOutBounce(float t)
  {
    if(t < 0.5f) return InBounce(t * 2f) * 0.5f;
    return 1f - InBounce((1f - t) * 2f) * 0.5f;
  }
}
}
