//  _____  ___ __  __                                                                                 
// |_   _|| __|\ \/ /                                                                                 
//   | |  | _|  >  <                                                                                  
//   |_|  |___|/_/\_\                                                                                 
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using System;
using UnityEngine;
#endregion

namespace MathBad
{
public static class TEX
{
  public static Texture2D Create(int width, int height,
                                 FilterMode filterMode = FilterMode.Bilinear,
                                 TextureFormat format = TextureFormat.RGBA32,
                                 bool mipMaps = false)
  {
    Texture2D tex = new Texture2D(width, height, format, mipMaps);
    tex.filterMode = filterMode;
    tex.Apply();
    return tex;
  }
}
public static class RTEX
{
  public static RenderTexture Create(int width, int height,
                                     FilterMode filterMode = FilterMode.Point,
                                     RenderTextureFormat format = RenderTextureFormat.ARGB32)
  {
    RenderTexture rt = new RenderTexture(width, height, 0, format);
    rt.filterMode = filterMode;
    rt.wrapMode = TextureWrapMode.Clamp;
    rt.enableRandomWrite = true;
    rt.autoGenerateMips = false;
    rt.useMipMap = false;
    rt.hideFlags = HideFlags.HideAndDontSave;
    rt.Create();
    return rt;
  }

  public static RenderTexture Create(Texture2D initTex,
                                     FilterMode filterMode = FilterMode.Point,
                                     RenderTextureFormat format = RenderTextureFormat.ARGB32)
  {
    RenderTexture rt = Create(initTex.width, initTex.height, filterMode, format);
    Graphics.CopyTexture(initTex, rt);
    return rt;
  }

  public static void LoadGLWorldPixelMatrix(RenderTexture rt, Vector2 texCoord, Vector2 right, Vector2 scale, Action callback)
  {
    RenderTexture.active = rt;

    GL.PushMatrix();
    GL.LoadPixelMatrix(0, rt.width, 0, rt.height);

    Matrix4x4 matrix = Matrix4x4.TRS(texCoord * rt.Size(), right.ToRotation(), scale);
    GL.MultMatrix(matrix);

    callback();

    GL.PopMatrix();

    RenderTexture.active = null;
  }
}
}
