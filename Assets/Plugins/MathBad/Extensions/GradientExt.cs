#region Usings
using System;
using UnityEngine;
using MathBad;
#endregion

public static class GradientExt
{
    public static void ReverseAlphaKeys(this Gradient gradient)
    {
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
        for(int i = 0; i < alphaKeys.Length; i++)
        {
            alphaKeys[i].time = 1f - alphaKeys[i].time;
        }
        Array.Reverse(alphaKeys);
        gradient.alphaKeys = alphaKeys;
    }

    public static void ReverseColorKeys(this Gradient gradient)
    {
        GradientColorKey[] colorKeys = gradient.colorKeys;
        for(int i = 0; i < colorKeys.Length; i++)
        {
            colorKeys[i].time = 1f - colorKeys[i].time;
        }
        Array.Reverse(colorKeys);

        gradient.colorKeys = colorKeys;
    }
    
    public static void ReverseAllKeys(this Gradient gradient)
    {
        gradient.ReverseAlphaKeys();
        gradient.ReverseColorKeys();
    }
}
