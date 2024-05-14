#region
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Unity.Mathematics.math;
#endregion

namespace MathBad
{
public static class TextureExt
{
    static Dictionary<TextureFormat, RenderTextureFormat> _renderTextureFormatLookup
        = new Dictionary<TextureFormat, RenderTextureFormat>
          {
              {TextureFormat.Alpha8, RenderTextureFormat.ARGB32},
              {TextureFormat.ARGB4444, RenderTextureFormat.ARGB4444},
              {TextureFormat.RGB24, RenderTextureFormat.ARGB32},
              {TextureFormat.RGBA32, RenderTextureFormat.ARGB32},
              {TextureFormat.ARGB32, RenderTextureFormat.ARGB32},
              {TextureFormat.RGB565, RenderTextureFormat.RGB565},
              {TextureFormat.R16, RenderTextureFormat.RHalf},
              {TextureFormat.DXT1, RenderTextureFormat.ARGB32},
              {TextureFormat.DXT5, RenderTextureFormat.ARGB32},
              {TextureFormat.RGBA4444, RenderTextureFormat.ARGB4444},
              {TextureFormat.BGRA32, RenderTextureFormat.ARGB32},
              {TextureFormat.RHalf, RenderTextureFormat.RHalf},
              {TextureFormat.RGHalf, RenderTextureFormat.RGHalf},
              {TextureFormat.RGBAHalf, RenderTextureFormat.ARGBHalf},
              {TextureFormat.RFloat, RenderTextureFormat.RFloat},
              {TextureFormat.RGFloat, RenderTextureFormat.RGFloat},
              {TextureFormat.RGBAFloat, RenderTextureFormat.ARGBFloat},
              {TextureFormat.RGB9e5Float, RenderTextureFormat.ARGBHalf},
              {TextureFormat.BC4, RenderTextureFormat.R8},
              {TextureFormat.BC5, RenderTextureFormat.RGHalf},
              {TextureFormat.BC6H, RenderTextureFormat.ARGBHalf},
              {TextureFormat.BC7, RenderTextureFormat.ARGB32}
          };
    static Dictionary<RenderTextureFormat, TextureFormat> _textureFormatLookup
        = new Dictionary<RenderTextureFormat, TextureFormat>
          {
              {RenderTextureFormat.ARGB4444, TextureFormat.ARGB4444},
              {RenderTextureFormat.ARGB32, TextureFormat.ARGB32},
              {RenderTextureFormat.RGB565, TextureFormat.RGB565},
              {RenderTextureFormat.RHalf, TextureFormat.RHalf},
              {RenderTextureFormat.RGHalf, TextureFormat.RGHalf},
              {RenderTextureFormat.ARGBHalf, TextureFormat.RGBAHalf},
              {RenderTextureFormat.RFloat, TextureFormat.RFloat},
              {RenderTextureFormat.RGFloat, TextureFormat.RGFloat},
              {RenderTextureFormat.ARGBFloat, TextureFormat.RGBAFloat},
              {RenderTextureFormat.R8, TextureFormat.BC4}
          };

    [MethodImpl(256)] public static int Length(this Texture2D texture) => texture.width * texture.height;
    [MethodImpl(256)] public static int Length(this RenderTexture rt) => rt.width * rt.height;
    [MethodImpl(256)] public static Int2 Size(this Texture2D texture) => new Int2(texture.width, texture.height);
    [MethodImpl(256)] public static Int2 Size(this RenderTexture rt) => new Int2(rt.width, rt.height);
    [MethodImpl(256)] public static Texture2D WithName(this Texture2D texture, string name)
    {
        texture.name = name;
        return texture;
    }

    [MethodImpl(256)] public static Color[] GetPixels(this Texture2D texture, Rect rect) => texture.GetPixels((int)rect.x, (int)rect.y,
                                                                                                              (int)rect.width, (int)rect.height);

    // Sprite
    //----------------------------------------------------------------------------------------------------
    public static Rect UVRect(this Sprite sprite)
    {
        Rect pixelRect = sprite.textureRect;

        float u = pixelRect.x / sprite.texture.width;
        float v = pixelRect.y / sprite.texture.height;
        float width = pixelRect.width / sprite.texture.width;
        float height = pixelRect.height / sprite.texture.height;

        Rect result = new Rect(u, v, width, height);
        return result;
    }

    public static Texture2D ToTexture2D(this Sprite sprite)
    {
        Texture2D result = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
        result.filterMode = sprite.texture.filterMode;
        result.Apply(sprite.texture.GetPixels(sprite.textureRect));
        return result;
    }

    public static Texture2D[] ToTexture2DArray(this Sprite[] sprites)
    {
        if(sprites.IsNullOrEmpty())
        {
            throw new NullReferenceException($"{nameof(sprites)} is null or empty.");
        }
        Texture2D[] results = new Texture2D[sprites.Length];
        for(int i = 0; i < sprites.Length; i++)
        {
            results[i] = sprites[i].ToTexture2D();
        }
        return results;
    }

    // Render Texture
    //----------------------------------------------------------------------------------------------------
    public static int BytesPerTexel(this Texture2D tex)
    {
        switch(tex.format)
        {
            // 1 byte per texel.
            case TextureFormat.Alpha8:
            case TextureFormat.R8:
                return 1;

            // 2 bytes per texel.
            case TextureFormat.ARGB4444:
            case TextureFormat.RGBA4444:
            case TextureFormat.R16:
            case TextureFormat.RG16:
            case TextureFormat.RG32:
                return 2;

            // 3 bytes per texel.
            case TextureFormat.RGB24:
                return 3;

            // 4 bytes per texel.
            case TextureFormat.ARGB32:
            case TextureFormat.RGBA32:
            case TextureFormat.RFloat:
                return 4;

            // 6 bytes per texel.
            case TextureFormat.RGB48:
                return 6;

            // 8 bytes per texel.
            case TextureFormat.RGBA64:
            case TextureFormat.RGFloat:
                return 8;

            // 16 bytes per texel.
            case TextureFormat.RGBAFloat:
                return 16;

            default:
                Debug.Log($"Unsupported texture format: {tex.format}");
                return -1;
        }
    }

    public static TextureFormat ToTextureFormat(this RenderTextureFormat format)
    {
        if(_textureFormatLookup.ContainsKey(format))
            return _textureFormatLookup[format];
        return TextureFormat.ARGB32;
    }
    public static RenderTextureFormat ToRenderTextureFormat(this TextureFormat format)
    {
        if(_renderTextureFormatLookup.ContainsKey(format))
            return _renderTextureFormatLookup[format];
        return RenderTextureFormat.ARGB32;
    }

    public static Texture2D ToTexture2D(this RenderTexture rt)
    {
        Texture2D texture = TEX.Create(rt.width, rt.height, rt.filterMode, rt.format.ToTextureFormat());
        texture.hideFlags = HideFlags.HideAndDontSave;

        RenderTexture.active = rt;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, rt.width, 0, rt.height);
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        GL.PopMatrix();
        RenderTexture.active = null;

        texture.Apply();
        return texture;
    }

    // Texture2D
    //----------------------------------------------------------------------------------------------------
    /// <summary>
    /// Set the textures pixels and apply.
    /// </summary>
    [MethodImpl(256)]
    public static void Apply(this Texture2D tex, Color[] colors)
    {
        tex.SetPixels(colors);
        tex.Apply();
    }

    /// <summary>
    /// Set the textures pixels and apply.
    /// </summary>
    [MethodImpl(256)]
    public static void Apply(this Texture2D tex, Color32[] colors)
    {
        tex.SetPixels32(colors);
        tex.Apply();
    }

    public static float widthRatio(this Texture2D texture) => texture.width._float() / texture.height;
    public static float heightRatio(this Texture2D texture) => texture.height._float() / texture.width;

    /// <summary>
    /// Resizes a texture using Graphics.Blit.
    /// </summary>
    public static Texture2D BlitResize(this Texture2D texture, int width, int height, FilterMode filterMode = FilterMode.Bilinear)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
        rt.filterMode = FilterMode.Bilinear;

        RenderTexture.active = rt;
        GL.Clear(false, true, new Color(1f, 1f, 1f, 0.0f));
        bool sRgbWrite = GL.sRGBWrite;
        GL.sRGBWrite = false;
        Graphics.Blit(texture, rt);

        texture.Reinitialize(width, height);
        texture.filterMode = filterMode;
        texture.ReadPixels(new Rect(0.0f, 0.0f, width, height), 0, 0);
        texture.Apply();

        RenderTexture.active = active;
        RenderTexture.ReleaseTemporary(rt);

        GL.sRGBWrite = sRgbWrite;
        return texture;
    }

    public static Texture2D GaussianBlur(this Texture2D texture, int radius)
    {
        int width = texture.width;
        int height = texture.height;
        Texture2D blurred = new Texture2D(width, height, texture.format, false);

        // kernel
        float[] kernel = CreateGaussianKernel(radius);
        int kernelSize = kernel.Length;
        int kernelRadius = kernelSize / 2;

        // apply
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                float sum = 0;
                Color blurredColor = Color.clear;

                for(int ky = -kernelRadius; ky <= kernelRadius; ky++)
                {
                    for(int kx = -kernelRadius; kx <= kernelRadius; kx++)
                    {
                        int px = clamp(x + kx, 0, width - 1);
                        int py = clamp(y + ky, 0, height - 1);

                        blurredColor += texture.GetPixel(px, py) * kernel[ky + kernelRadius] * kernel[kx + kernelRadius];
                        sum += kernel[ky + kernelRadius] * kernel[kx + kernelRadius];
                    }
                }

                blurred.SetPixel(x, y, blurredColor / sum);
            }
        }

        blurred.Apply();
        return blurred;
        //--------------------------------------------------
        float[] CreateGaussianKernel(int r)
        {
            int size = 2 * r + 1;
            float[] k = new float[size];
            float sigma = r / 2.0f;
            float sum = 0.0f;

            for(int i = 0; i < size; i++)
            {
                int x = i - r;
                k[i] = exp(-0.5f * x * x / (sigma * sigma)) / (sqrt(2.0f * PI) * sigma);
                sum += k[i];
            }

            // Normalize the kernel
            for(int i = 0; i < size; i++)
            {
                k[i] /= sum;
            }

            return k;
        }
    }

    // Fill
    //----------------------------------------------------------------------------------------------------
    public static Texture2D Fill(this Texture2D texture, Color color) => texture.Fill((Color32)color);
    public static Texture2D Fill(this Texture2D texture, Color32 color)
    {
        Color32[] colors = new Color32[texture.width * texture.height];

        for(int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }

        texture.Apply(colors);
        return texture;
    }

    public static Texture2D GadientFill(this Texture2D texture, Gradient gradient,
                                        EaseType easeType = EaseType.Linear)
    {
        Color32[] colors = new Color32[texture.width * texture.height];

        for(int i = 0; i < colors.Length; i++)
        {
            Int2 xy = mathi.ixy(i, texture.width);
            float t = mathi.unlerp(xy.x, texture.width);
            colors[i] = (Color32)gradient.Evaluate(EASE.Evaluate(t, easeType));
        }

        texture.Apply(colors);
        return texture;
    }

    public static Texture2D GadientFillNorthSouth(this Texture2D texture,
                                          Gradient t, Gradient b,
                                          EaseType easeType = EaseType.Linear)
    {
        Color32[] colors = new Color32[texture.width * texture.height];

        for(int i = 0; i < colors.Length; i++)
        {
            Int2 xy = mathi.ixy(i, texture.width);
            float x_lerp = mathi.unlerp(xy.x, texture.width);
            Color c0 = t.Evaluate(x_lerp);
            Color c1 = b.Evaluate(x_lerp);

            float y_lerp = mathi.unlerp(xy.y, texture.height);
            Color c = Color.Lerp(c1, c0, EASE.Evaluate(y_lerp, easeType));
            colors[i] = (Color32)c;
        }

        texture.Apply(colors);
        return texture;
    }
    public static Texture2D GadientFillEastWest(this Texture2D texture,
                                                  Gradient t, Gradient b,
                                                  EaseType easeType = EaseType.Linear)
    {
        Color32[] colors = new Color32[texture.width * texture.height];

        for(int i = 0; i < colors.Length; i++)
        {
            Int2 xy = mathi.ixy(i, texture.width);
            float y_lerp = mathi.unlerp(xy.y, texture.height);
            Color c0 = t.Evaluate(y_lerp);
            Color c1 = b.Evaluate(y_lerp);

            float x_lerp = mathi.unlerp(xy.x, texture.width);
            Color c = Color.Lerp(c1, c0, EASE.Evaluate(x_lerp, easeType));
            colors[i] = (Color32)c;
        }

        texture.Apply(colors);
        return texture;
    }
    public static Texture2D GadientFillHV(this Texture2D texture,
                                          Gradient h, Gradient v,
                                          EaseType easeType = EaseType.Linear)
    {
        Color32[] colors = new Color32[texture.width * texture.height];

        for(int y = 0, i = 0; y < texture.height; y++)
        for(int x = 0; x < texture.width; x++, i++)
        {
            float x_lerp = mathi.unlerp(x, texture.width);
            Color c0 = h.Evaluate(x_lerp);

            float y_lerp = mathi.unlerp(y, texture.height);
            Color c1 = v.Evaluate(y_lerp);

            float blend = (x_lerp + (1f - y_lerp)) * 0.5f;
            Color c = Color.Lerp(c0, c1, EASE.Evaluate(blend, easeType));
            colors[i] = (Color32)c;
        }

        texture.Apply(colors);
        return texture;
    }

    // Checker Pattern
    //----------------------------------------------------------------------------------------------------
    public static Texture2D CheckerFill(this Texture2D texture, int tileWidth, int tileHeight, Color light, Color dark)
    {
        Color32[] colors = new Color32[texture.width * texture.height];
        bool cx = true;

        for(int x = 0, i = 0; x < texture.height; x++)
        {
            if(x % tileWidth == 0) cx = !cx;
            bool cy = true;
            for(int y = 0; y < texture.width; y++, i++)
            {
                if(y % tileHeight == 0) cy = !cy;
                bool checker = cy && cx || !cy && !cx;
                colors[i] = checker ? light : dark;
            }
        }

        texture.Apply(colors);
        return texture;
    }

    // Flood Fill
    //----------------------------------------------------------------------------------------------------
    public static Texture2D FloodFill(this Texture2D texture, int x, int y, Color fillColor)
    {
        Color color = texture.GetPixel(x, y);
        texture.Apply(FloodFillRecurssive(color, fillColor));

        return texture;
        //--------------------------------------------------
        Color32[] FloodFillRecurssive(Color targetColor, Color replacementColor)
        {
            int width = texture.width;
            Color32[] colors = texture.GetPixels32();

            HashSet<Int2> visited = new HashSet<Int2>();
            Queue<Int2> queue = new Queue<Int2>();
            queue.Enqueue(new Int2(x, y));

            while(queue.Count > 0)
            {
                Int2 pixel = queue.Dequeue();
                int i = mathi.yxi(pixel.x, pixel.y, texture.height);
                if(!visited.Add(pixel))
                    continue;

                if(colors[i] != targetColor)
                    continue;

                colors[i] = replacementColor;

                if(pixel.x > 0) queue.Enqueue(new Int2(pixel.x - 1, pixel.y));
                if(pixel.x < texture.width - 1) queue.Enqueue(new Int2(pixel.x + 1, pixel.y));
                if(pixel.y > 0) queue.Enqueue(new Int2(pixel.x, pixel.y - 1));
                if(pixel.y < texture.height - 1) queue.Enqueue(new Int2(pixel.x, pixel.y + 1));
            }

            return colors;
        }
    }
}
}
