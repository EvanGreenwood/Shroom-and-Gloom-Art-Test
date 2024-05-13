//  __  __    _    _____  _  _                                                                        
// |  \/  |  /_\  |_   _|| || |                                                                       
// | |\/| | / _ \   | |  | __ |                                                                       
// |_|  |_|/_/ \_\  |_|  |_||_|                                                                       
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
#endregion

namespace MathBad
{
public static class mathi
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int2 ixy(int i, int width) => new int2(i % width, i / width);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int xyi(int x, int y, int height) => x * height + y;
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int yxi(int x, int y, int width) => x + y * width;
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int xyzi(int x, int y, int z, int width, int height) => x + y * width + z * width * height;
  public static float unlerp(int index, int count) => math.unlerp(0f, count - 1, index);
}
public static partial class MATH
{
  public const float TAU = 6.28318548f;

  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Clamp01(float x) => math.clamp(x, 0.0f, 1.0f);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Clamp01(double x) => math.clamp(x, 0.0d, 1.0d);

  // Index Conversion 1D, 2D & 3D
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int2 I_XY(int i, int width) => new int2(i % width, i / width);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int XY_I(int x, int y, int width) => x * width + y;
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int YX_I(int x, int y, int height) => x + y * height;
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int XYZ_I(int x, int y, int z, int width, int height) => x + y * width + z * width * height;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int3 I_XYZ(int i, int width, int height)
  {
    int z = i / (width * height);
    i -= z * width * height;
    int y = i / width;
    int x = i % width;
    return new int3(x, y, z);
  }

  // Sin
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Sin01(float t) => (math.sin(t) + 1.0f) * 0.5f;
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Cost01(float t) => (math.cos(t) + 1.0f) * 0.5f;

  // Min
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Min(int a, int b) => math.min(a, b);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Min(int a, int b, int c) => math.min(math.min(a, b), c);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Min(int a, int b, int c, int d) => math.min(math.min(math.min(a, b), c), d);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Min(float a, float b) => math.min(a, b);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Min(float a, float b, float c) => math.min(math.min(a, b), c);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Min(float a, float b, float c, float d) => math.min(math.min(math.min(a, b), c), d);

  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Min(Vector2 v) => Min(v.x, v.y);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Min(Vector3 v) => Min(v.x, v.y, v.z);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Min(float2 v) => Min(v.x, v.y);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Min(float3 v) => Min(v.x, v.y, v.z);

  // Max
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Max(int a, int b) => math.max(a, b);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Max(int a, int b, int c) => math.max(math.max(a, b), c);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Max(float a, float b) => math.max(a, b);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Max(float a, float b, float c) => math.max(math.max(a, b), c);

  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float CMax(Vector2 v) => Max(v.x, v.y);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float CMax(Vector3 v) => Max(v.x, v.y, v.z);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float CMax(float2 v) => Max(v.x, v.y);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float CMax(float3 v) => Max(v.x, v.y, v.z);

  // Lerp
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Lerp(float a, float b, float x) => math.lerp(a, b, x);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 Lerp(Vector2 a, Vector2 b, float x) => Vector2.Lerp(a, b, x);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Lerp(Vector3 a, Vector3 b, float x) => Vector3.Lerp(a, b, x);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 Lerp(float2 a, float2 b, float x) => math.lerp(a, b, x);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 Lerp(float3 a, float3 b, float x) => math.lerp(a, b, x);

  // Unlerp
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Unlerp(float a, float b, float x) => math.unlerp(a, b, x);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 Unlerp(Vector2 a, Vector2 b, Vector2 x) => math.unlerp(a, b, x)._Vec2();
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Unlerp(Vector3 a, Vector3 b, Vector3 x) => math.unlerp(a, b, x)._Vec3();
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 Unlerp(float2 a, float2 b, float2 x) => math.unlerp(a, b, x);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 Unlerp(float3 a, float3 b, float3 x) => math.unlerp(a, b, x);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Unlerp01(float a, float b, float x) => math.unlerp(a, b, x);

  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Remap(float a, float b, float c, float d, float x) => math.remap(a, b, c, d, x);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 Remap(float2 a, float2 b, float2 c, float2 d, float2 x) => math.remap(a, b, c, d, x);
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 Remap(float3 a, float3 b, float3 c, float3 d, float3 x) => math.remap(a, b, c, d, x);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float EvaluateDampingCurve(float x, float t)
  {
    float b = 1f - Mathf.Pow(x.Clamp01(), Mathf.Lerp(2f, 0.25f, t));
    return b * b * b;
  }

  //                    _       
  //  __ _  _ _   __ _ | | ___  
  // / _` || ' \ / _` || |/ -_) 
  // \__,_||_||_|\__, ||_|\___| 
  //             |___/          
  //----------------------------------------------------------------------------------------------------
  /// <returns> 0f ... 180f degrees. </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Angle(float2 from, float2 to) => math.degrees(math.acos(math.clamp(math.dot(math.normalize(from), math.normalize(to)), -1.0f, 1.0f)));

  /// <returns> 0f ... 180f degrees. </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Angle(float3 from, float3 to) => math.degrees(math.acos(math.clamp(math.dot(math.normalize(from), math.normalize(to)), -1.0f, 1.0f)));

  // Signed Angle
  //----------------------------------------------------------------------------------------------------
  /// <returns> -180f ... 180f degrees. </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float SignedAngle(float2 from, float2 to) => Angle(from, to) * math.sign(from.x * to.y - from.y * to.x);

  /// <returns> -180f ... 180f degrees. </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float SignedAngle(float3 a, float3 b, float3 axis) => Angle(a, b) * math.sign(math.dot(axis, math.cross(a, b)));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float ClampAngle(float angle, float min, float max)
  {
    if(angle < -360f) angle += 360f;
    if(angle > 360f) angle -= 360f;
    return Mathf.Clamp(angle, min, max);
  }

  /// <returns> 0 to 360 </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float Normalize_360(float angle)
  {
    angle %= 360f;
    if(angle < 0) { angle += 360f; }
    return angle;
  }

  /// <returns> -180 to 180 </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float Normalize_180(float angle)
  {
    angle = Normalize_360(angle);
    if(angle > 180f) { angle -= 360f; }
    return angle;
  }

  /// <summary>
  /// Calculates the angle in degrees between two vectors, v1 and v2.
  /// </summary>
  /// <param name="v1">First vector.</param>
  /// <param name="v2">Second vector.</param>
  /// <returns>( 0 -> 180 ) The angle in degrees between v1 and v2.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float angle(float3 v1, float3 v2) => math.degrees(math.acos(math.clamp(math.dot(v1, v2) / math.sqrt(math.lengthsq(v1) * math.lengthsq(v2)), -1f, 1f)));

  /// <summary>
  /// Calculates the signed angle in degrees between two vectors, v1 and v2, relative to a specified normal vector.
  /// </summary>
  /// <param name="v1">First vector.</param>
  /// <param name="v2">Second vector.</param>
  /// <param name="normal">Normal vector.</param>
  /// <returns>( -180 -> 180 ) The signed angle in degrees between v1 and v2, relative to the normal vector.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float signedangle(float3 v1, float3 v2, float3 normal) => angle(v1, v2) * math.dot(normal, math.cross(v1, v2)) < 0 ? -1f : 1f;

  //BarycentricCoords
  /// <summery> Calculates the barycentric coordinates of a float2 "p" relative to a triangle defined by vertices t0, t1, and t2 in 2D space. </summery>
  /// <returns> The result is returned as a 3D vector with the barycentric coordinates "u", "v", and "w". </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float3 barycentric(float2 p, float2 t0, float2 t1, float2 t2)
  {
    float2 v0 = t1 - t0, v1 = t2 - t0, v2 = p - t0;
    float d00 = math.dot(v0, v0), d01 = math.dot(v0, v1), d11 = math.dot(v1, v1), d20 = math.dot(v2, v0), d21 = math.dot(v2, v1);
    float dn = d00 * d11 - d01 * d01;
    float v = (d11 * d20 - d01 * d21) / dn,
          w = (d00 * d21 - d01 * d20) / dn,
          u = 1f - v - w;
    return new float3(u, v, w);
  }

  /// <summery> Calculates the barycentric coordinates of a float3 "p" relative to a triangle defined by vertices t0, t1, and t2 in 3D space. </summery>
  /// <returns> The result is returned as a 3D vector with the barycentric coordinates "u", "v", and "w". </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float3 barycentric(float3 p, float3 t0, float3 t1, float3 t2)
  {
    float3 v0 = t1 - t0, v1 = t2 - t0, v2 = p - t0;
    float d00 = math.dot(v0, v0), d01 = math.dot(v0, v1), d11 = math.dot(v1, v1), d20 = math.dot(v2, v0), d21 = math.dot(v2, v1);
    float dn = d00 * d11 - d01 * d01;
    float v = (d11 * d20 - d01 * d21) / dn,
          w = (d00 * d21 - d01 * d20) / dn,
          u = 1f - v - w;
    return new float3(u, v, w);
  }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  /// <summery> Calculates the world position of barycentric coordinates u,v and w from triangle vertex positions t0, t1 and t2 in 3D space. </summery>
  /// <returns> The result is returned as a 3D vector with the barycentric coordinates uvw. </returns>
  public static float3 barycentrictoworld(float3 t0, float3 t1, float3 t2, float3 uvw) => uvw.x * t0 + uvw.y * t1 + uvw.z * t2;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  /// <summery> Calculates the world position of barycentric coordinates u,v and w from triangle vertex positions t0, t1 and t2 in 3D space. </summery>
  /// <returns> The result is returned as a 3D vector with the barycentric coordinates u, v, and w. </returns>
  public static float3 barycentrictoworld(float3 t0, float3 t1, float3 t2, float u, float v, float w) => u * t0 + v * t1 + w * t2;

  // Circle
  //----------------------------------------------------------------------------------------------------
  public static Vector3 GetPointOnCircle(float radius, float angle)
    => new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * radius, Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 0);

  public static Vector3 GetPointOnCircle(Vector3 center, float radius, Vector3 normal, float angle)
    => center + Quaternion.LookRotation(normal) * new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * radius, Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 0);

  public static Vector3 GetPointOnCircleXZ(float radius, float angle)
    => new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * radius, 0f, Mathf.Cos(angle * Mathf.Deg2Rad) * radius);

  public static Vector3 GetPointOnUnitCircle(float angle, Vector3 normal)
    => Quaternion.LookRotation(normal) * new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
  public static Vector3 CircleLerp(Vector3 center, Vector3 normal, float radius, float t)
    => center + Quaternion.LookRotation(normal) * new Vector3(Mathf.Sin(t * 360f * Mathf.Deg2Rad) * radius, Mathf.Cos(t * 360f * Mathf.Deg2Rad) * radius, 0);

  /// <summary> Get positions on a circle </summary>
  public static Vector2[] GetPointsOnCircle(Vector2 origin, float radius, int resolution)
  {
    Vector2[] points = new Vector2[resolution];

    float slice = 2 * Mathf.PI / resolution;

    for(int i = 0; i < resolution; i++)
    {
      float angle = slice * i;
      float newX = (float)(origin.x + radius * Mathf.Cos(angle));
      float newY = (float)(origin.y + radius * Mathf.Sin(angle));
      points[i] = new Vector2(newX, newY);
    }

    return points;
  }
  // Parabola
  //----------------------------------------------------------------------------------------------------
  /// <summary> Get a point at t 0f..1f on a parabola </summary>
  public static Vector2 SampleParabola2(Vector2 start, Vector2 end, float maxHeight, float lerp, out float height)
  {
    Vector2 mid = Vector2.Lerp(start, end, lerp);
    height = Evaluate(lerp);
    return new Vector2(mid.x, height + Mathf.Lerp(start.y, end.y, lerp));
    float Evaluate(float x) => -4 * maxHeight * x * x + 4 * maxHeight * x;
  }

  /// <summary> Get a position at t 0f..1f  on a parabola </summary>
  public static Vector3 SampleParabola3(Vector3 v0, Vector3 v1, float maxHeight, float lerp)
  {
    Vector3 mid = Vector3.Lerp(v0, v1, lerp);
    return new Vector3(mid.x, Evaluate(lerp) + Mathf.Lerp(v0.y, v1.y, lerp), mid.z);
    float Evaluate(float x) => -4 * maxHeight * x * x + 4 * maxHeight * x;
  }

  /// <summary> Get positions on a circle </summary>
  public static Vector3[] GetPointsOnCircle3(float radius, int resolution) => GetPointsOnCircle3(Vector3.zero, Vector3.forward, Vector3.up, radius, resolution);
  /// <summary> Get positions on a circle </summary>
  public static Vector3[] GetPointsOnCircle3(Vector3 origin, Vector3 normal, Vector3 up, float radius, int resolution)
  {
    Vector3[] vertices = new Vector3[resolution];
    Vector3 tangent = Vector3.Cross(normal, -up);
    for(int i = 0; i < resolution; i++) vertices[i] = origin + Quaternion.AngleAxis(i / (float)resolution * 360f, normal) * tangent * radius;

    return vertices;
  }
  // Arc
  //----------------------------------------------------------------------------------------------------
  public static Vector2 SampleArc2(float radius, float t, float maxAngle)
  {
    float rad = Mathf.Deg2Rad * t * maxAngle;

    float x = radius * Mathf.Sin(rad);
    float y = radius * Mathf.Cos(rad);

    return new Vector2(x, y);
  }

  public static Vector3 SambleArc3(float radius, float t, float maxAngle)
  {
    float rad = Mathf.Deg2Rad * t * maxAngle;

    float x = radius * Mathf.Sin(rad);
    float z = radius * Mathf.Cos(rad);

    return new Vector3(x, 0f, z);
  }

  // Shape
  //----------------------------------------------------------------------------------------------------
  //  ___                           _                                                                   
  // | _ ) _ _  ___  ___ ___  _ _  | |_   __ _  _ __   ___                                              
  // | _ \| '_|/ -_)(_-</ -_)| ' \ | ' \ / _` || '  \ (_-<                                              
  // |___/|_|  \___|/__/\___||_||_||_||_|\__,_||_|_|_|/__/                                              
  //                                                                                                    
  //----------------------------------------------------------------------------------------------------
  static void Swap<T>(ref T t0, ref T t1)
  {
    T temp = t1;
    t1 = t0;
    t0 = temp;
  }
  //  ___  ___   
  // |_  )|   \  
  //  / / | |) | 
  // /___||___/  
  //             
  /// <summary> Get points along a line, using Bresenhams algorithm in 2D </summary>
  public static void PointLine2(List<int2> line, int2 a, int2 b) => PointLine2(line, a.x, a.y, b.x, b.y);
  public static void PointLine2(List<int2> line, Int2 a, Int2 b) => PointLine2(line, a.x, a.y, b.x, b.y);
  /// <summary> Get points along a line, using Bresenhams algorithm in 2D </summary>
  public static void PointLine2(List<int2> line, int x0, int y0, int x1, int y1)
  {
    bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
    if(steep)
    {
      Swap(ref x0, ref y0);
      Swap(ref x1, ref y1);
    }
    if(x0 > x1)
    {
      Swap(ref x0, ref x1);
      Swap(ref y0, ref y1);
    }

    int dx = x1 - x0, dy = Mathf.Abs(y1 - y0);
    int error = dx / 2;
    int yStep = y0 < y1 ? 1 : -1;
    int y = y0;
    for(int x = x0; x <= x1; x++)
    {
      line.Add(new int2(steep ? y : x, steep ? x : y));
      error -= dy;
      if(error < 0)
      {
        y += yStep;
        error += dx;
      }
    }
  }

  // Bresenhams 2D Multi Point Line with width
  /// <summary> Get points along a segmented line, using Bresenhams algorithm in 2D </summary>
  public static List<int2> PointLine2(int width, params int2[] points)
  {
    if(points.Length == 0) return new List<int2>();

    List<int2> line = new List<int2>();
    for(int i = 0; i < points.Length - 1; i++) line.AddRange(PointLine2(points[i], points[i + 1], width));

    return line;
  }

  // Bresenhams 2D with line width
  /// <summary> Get points along a line, using Bresenhams algorithm in 2D with thickness </summary>
  public static List<int2> PointLine2(int2 a, int2 b, int width) => PointLine2(a.x, a.y, b.x, b.y, width);
  /// <summary> Get points along a line, using Bresenhams algorithm in 2D with line width </summary>
  public static List<int2> PointLine2(int x0, int y0, int x1, int y1, int width)
  {
    int halfWidth = width.Half();
    List<int2> line = new List<int2>();

    bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
    if(steep)
    {
      Swap(ref x0, ref y0);
      Swap(ref x1, ref y1);
    }
    if(x0 > x1)
    {
      Swap(ref x0, ref x1);
      Swap(ref y0, ref y1);
    }

    int dx = x1 - x0, dy = Mathf.Abs(y1 - y0);
    int error = dx / 2;
    int yStep = y0 < y1 ? 1 : -1;
    int y = y0;

    for(int x = x0; x <= x1; x++)
    {
      for(int i = 0; i <= halfWidth; i++)
      {
        int j = (int)Mathf.Round(Mathf.Sqrt(halfWidth * halfWidth - i * i));
        line.Add(new int2(steep ? y + i : x + i, steep ? x + j : y + j));
        line.Add(new int2(steep ? y - i : x - i, steep ? x - j : y - j));
        line.Add(new int2(steep ? y + i : x - i, steep ? x + j : y - j));
        line.Add(new int2(steep ? y - i : x + i, steep ? x - j : y + j));
      }

      error -= dy;
      if(error < 0)
      {
        y += yStep;
        error += dx;
      }
    }

    return line;
  }

  //  ____ ___   
  // |__ /|   \  
  //  |_ \| |) | 
  // |___/|___/  
  //             
  /// <summary> Get positions along a line, using Bresenhams algorithm in 3D </summary>
  public static IList<int3> PointLine3(int x0, int y0, int z0, int x1, int y1, int z1)
  {
    List<int3> line = new List<int3>();

    bool steepXY = math.abs(y1 - y0) > math.abs(x1 - x0);
    if(steepXY)
    {
      Swap(ref x0, ref y0);
      Swap(ref x1, ref y1);
    }

    bool steepXZ = math.abs(z1 - z0) > math.abs(x1 - x0);
    if(steepXZ)
    {
      Swap(ref x0, ref z0);
      Swap(ref x1, ref z1);
    }

    int dx = math.abs(x1 - x0), dy = math.abs(y1 - y0), dz = math.abs(z1 - z0);

    int errorXY = dx / 2, errorXZ = dx / 2;

    int stepX = x0 > x1 ? -1 : 1;
    int stepY = y0 > y1 ? -1 : 1;
    int stepZ = z0 > z1 ? -1 : 1;

    int y = y0, z = z0;

    // Check if the end of the line.
    for(int x = x0; x != x1; x += stepX)
    {
      int xCopy = x, yCopy = y, zCopy = z;

      if(steepXZ) Swap(ref xCopy, ref zCopy);
      if(steepXY) Swap(ref xCopy, ref yCopy);

      line.Add(new int3(xCopy, yCopy, zCopy));

      errorXY -= dy;
      errorXZ -= dz;

      if(errorXY < 0)
      {
        y += stepY;
        errorXY += dx;
      }

      if(errorXZ < 0)
      {
        z += stepZ;
        errorXZ += dx;
      }
    }

    return line;
  }
}
}
