#region
using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
#endregion

namespace MathBad
{
public static partial class MathExt
{
    // Half
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Half(this int x) => x / 2;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Half(this float x) => x * 0.5f;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Half(this double x) => x * 0.5f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 Half(this float2 v) => v * 0.5f;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 Half(this float3 v) => v * 0.5f;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int2 Half(this int2 v) => v / 2;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int3 Half(this int3 v) => v / 2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 Half(this Vector2 v) => v * 0.5f;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Half(this Vector3 v) => v * 0.5f;

    // Squared
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Squared(this int x) => x * x;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Squared(this float x) => x * x;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Squared(this double x) => x * x;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 Squared(this float2 v) => new float2(v.x * v.x, v.y * v.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 Squared(this float3 v) => new float3(v.x * v.x, v.y * v.y, v.z * v.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int2 Squared(this int2 v) => new int2(v.x * v.x, v.y * v.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int3 Squared(this int3 v) => new int3(v.x * v.x, v.y * v.y, v.z * v.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 Squared(this Vector2 v) => new Vector2(v.x * v.x, v.y * v.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Squared(this Vector3 v) => new Vector3(v.x * v.x, v.y * v.y, v.z * v.z);

    // Absolute
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Abs(this int x) => math.abs(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Abs(this float x) => math.abs(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Abs(this double x) => math.abs(x);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 Abs(this float2 v) => math.abs(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 Abs(this float3 v) => math.abs(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int2 Abs(this int2 v) => math.abs(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int3 Abs(this int3 v) => math.abs(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 Abs(this Vector2 v) => math.abs(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Abs(this Vector3 v) => math.abs(v);

    // FloorToInt
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int FloorToInt(this float x) => (int)math.floor(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int FloorToInt(this double x) => (int)math.floor(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int3 FloorToInt(this float3 v) => math.floor(v)._int3();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int2 FloorToInt(this float2 v) => math.floor(v)._int2();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2Int FloorToInt(this Vector2 v) => math.floor(v)._Vec2i();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3Int FloorToInt(this Vector3 v) => math.floor(v)._Vec3i();

    // Clamp
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Clamp(this int x, int min, int max) => math.clamp(x, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Clamp(this float x, float min, float max) => math.clamp(x, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Clamp(this double x, double min, double max) => math.clamp(x, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 Clamp(this float2 v, float2 min, float2 max) => math.clamp(v, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 Clamp(this float3 v, float3 min, float3 max) => math.clamp(v, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int2 Clamp(this int2 v, int2 min, int2 max) => math.clamp(v, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int3 Clamp(this int3 v, int3 min, int3 max) => math.clamp(v, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 Clamp(this Vector2 v, float min, float max) => math.clamp(v, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max) => math.clamp(v, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Clamp(this Vector3 v, float min, float max) => math.clamp(v, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max) => math.clamp(v, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Clamp01(this float x) => math.clamp(x, 0.0f, 1.0f);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Clamp01(this double x) => math.clamp(x, 0.0d, 1.0d);

    // Odd or Even
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsEven(this int v) => v % 2 == 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOdd(this int v) => v % 2 == 1;

    // Normalize Safe
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 NormalizeSafe(this float2 v) => math.normalizesafe(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 NormalizeSafe(this float3 v) => math.normalizesafe(v);

    // Round To Int
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int RoundToInt(this float x) => Mathf.RoundToInt(x);

    // Length
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Magnitude(this float2 v) => math.length(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Magnitude(this float3 v) => math.length(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Magnitude(this int2 v) => math.length(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Magnitude(this int3 v) => math.length(v);

    // Squared Length
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float MagnitudeSq(this float2 v) => math.lengthsq(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float MagnitudeSq(this float3 v) => math.lengthsq(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float MagnitudeSq(this int2 v) => math.lengthsq(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float MagnitudeSq(this int3 v) => math.lengthsq(v);

    // Floating-Point Comparison
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Approx(this float x, float target) => x >= target - math.EPSILON && x <= target + math.EPSILON;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Approx(this Vector2 v, float target) => v.x.Approx(target) && v.y.Approx(target);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Approx(this Vector3 v, float target) => v.x.Approx(target) && v.y.Approx(target) && v.z.Approx(target);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Approx(this float2 v, float target) => v.x.Approx(target) && v.y.Approx(target);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Approx(this float3 v, float target) => v.x.Approx(target) && v.y.Approx(target) && v.z.Approx(target);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Approx(this float3 v, float3 target) => v.x.Approx(target.x) && v.y.Approx(target.y) && v.z.Approx(target.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Approx(this Vector3 v, Vector3 target) => v.x.Approx(target.x) && v.y.Approx(target.y) && v.z.Approx(target.z);

    // Min
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Min(this int a, int b) => math.min(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Min(this int a, int b, int c) => math.min(math.min(a, b), c);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Min(this float a, float b) => math.min(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Min(this float a, float b, float c) => math.min(math.min(a, b), c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float CMin(this Vector2 v) => math.cmin(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float CMin(this Vector3 v) => math.cmin(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float CMin(this float2 v) => math.cmin(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float CMin(this float3 v) => math.cmin(v);

    // Max
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Max(this int a, int b) => math.max(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Max(this int a, int b, int c) => math.max(math.max(a, b), c);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Max(this float a, float b) => math.max(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Max(this float a, float b, float c) => math.max(math.max(a, b), c);

    // Truncate
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Truncate(this float v) => (int)Math.Truncate(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Truncate(this double v) => (int)Math.Truncate(v);

    // Component max
    //----------------------------------------------------------------------------------------------------
    /// <returns> Returns the largest component (x, y) </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float CMax(this Vector2 v) => math.cmax(v);
    /// <returns> Returns the largest component (x, y, z) </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float CMax(this Vector3 v) => math.cmax(v);
    /// <returns> Returns the largest component (x, y) </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float CMax(this float2 v) => math.cmax(v);
    /// <returns> Returns the largest component (x, y, z) </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float CMax(this float3 v) => math.cmax(v);

    // // With Component
    // //----------------------------------------------------------------------------------------------------//
    // // X
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.y);
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2Int WithX(this Vector2Int v, int x) => new Vector2Int(x, v.y);
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3Int WithX(this Vector3Int v, int x) => new Vector3Int(x, v.y, v.z);
    // // Y
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.x, y);
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 WithY(this float3 v, float y) => new float3(v.x, y, v.z);
    // // Z
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 WithZ(this float3 v, float z) => new float3(v.x, v.y, z);

    // With component absolute
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 WithAbsX(this Vector2 v) => new Vector2(v.x.Abs(), v.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 WithAbsX(this Vector3 v) => new Vector3(v.x.Abs(), v.y, v.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 WithAbsY(this Vector2 v) => new Vector2(v.x, v.y.Abs());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 WithAbsY(this Vector3 v) => new Vector3(v.x, v.y.Abs(), v.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 WithAbsZ(this Vector3 v) => new Vector3(v.x, v.y, v.z.Abs());

    // Round
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Round(this int x, int nearest) => nearest == 0 ? 0 : (int)(Mathf.Round((float)x / nearest) * nearest);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Round(this float x, float nearest) => nearest == 0f ? 0f : Mathf.Round(x / nearest) * nearest;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 Round(this Vector2 v, float round) => round == 0.0f ? Vector2.zero : new Vector2(v.x.Round(round), v.y.Round(round));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Round(this Vector3 v, float round) => round == 0.0f ? Vector3.zero : new Vector3(v.x.Round(round), v.y.Round(round), v.z.Round(round));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 Round(this float2 vector, float round) => round == 0.0f ? float2.zero : new float2(vector.x.Round(round), vector.y.Round(round));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 Round(this float3 v, float round) => round == 0.0f ? float3.zero : new float3(v.x.Round(round), v.y.Round(round), v.z.Round(round));

    // Round Ceil
    //----------------------------------------------------------------------------------------------------//
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int RoundCeil(this int value, int round) => round == 0 ? 0 : (int)Mathf.Ceil((float)value / round) * round;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float RoundCeil(this float value, float round) => round == 0 ? 0.0f : Mathf.Ceil(value / round) * round;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 RoundCeil(this Vector2 value, float round) => new Vector2(value.x.RoundCeil(round), value.y.RoundCeil(round));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 RoundCeil(this Vector3 value, float round) => new Vector3(value.x.RoundCeil(round), value.y.RoundCeil(round), value.z.RoundCeil(round));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 RoundCeil(this float2 value, float round) => new float2(value.x.RoundCeil(round), value.y.RoundCeil(round));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 RoundCeil(this float3 value, float round) => new float3(value.x.RoundCeil(round), value.y.RoundCeil(round), value.z.RoundCeil(round));

    // Wrap
    //----------------------------------------------------------------------------------------------------//
    /// <summary> Wrap n inside min..max, increment by n </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Wrap(this float v, float min, float max) => v < min ? max - v % max : v % max;
    /// <summary> Wrap n inside min..max, increment by n </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Wrap(this int v, int min, int max) => v < min ? max - v % max : v % max;
    /// <summary> Wrap n inside 0f..1f, increment by n </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Wrap01(this float v) => v < 0 ? 1f - v % 1f : v % 1f;
    /// <summary> Wrap n inside 0..1, increment by n </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Wrap01(this int v) => v < 0 ? 1 - v % 1 : v % 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float ToPercent(this float v) => v * 100f;
    // Rotation
    //----------------------------------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float AngleDelta(this float a, float b) => Mathf.DeltaAngle(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float ToAngle(this Vector2 v) => math.atan2(v.y, v.x) * Mathf.Rad2Deg;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Rotate(this Vector2 v, float angle)
    {
        math.sincos(angle * Mathf.Deg2Rad, out float sin, out float cos);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Rotate(this Vector3 v, float angle) => Quaternion.AngleAxis(angle, Vector3.up) * v;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Rotate(this Vector3 v, float angle, Vector3 up) => Quaternion.AngleAxis(angle, up) * v;

    public static Vector2 Clerp(this Vector2 a, Vector2 b, float t)
    {
        float r = Mathf.Lerp(a.magnitude, b.magnitude, t);
        float angle = Mathf.Lerp(Mathf.Atan2(a.y, a.x), Mathf.Atan2(b.y, b.x), t);
        return new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float RotateTowards(this Vector2 v, Vector2 target) => Mathf.Atan2(target.y - v.y, target.x - v.x) * (180f / Mathf.PI);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Quaternion ToRotation(this Vector2 v, float angleOffset = 0.0f) => Quaternion.Euler(0, 0, v.ToAngle() + angleOffset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 up(this quaternion rotation) => math.rotate(rotation, math.up());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 down(this quaternion rotation) => math.rotate(rotation, math.down());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 left(this quaternion rotation) => math.rotate(rotation, math.left());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 right(this quaternion rotation) => math.rotate(rotation, math.right());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 forward(this quaternion rotation) => math.rotate(rotation, math.forward());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 back(this quaternion rotation) => math.rotate(rotation, math.back());

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 heading(this float3 v0, float3 v1) => v1 - v0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 direction(this float3 v0, float3 v1) => v0.heading(v1).normalize();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 normalize(this float2 v) => math.normalize(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 normalize(this float3 v) => math.normalize(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 normalizesafe(this float2 v) => math.normalizesafe(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 normalizesafe(this float3 v) => math.normalizesafe(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float length(this float2 v) => math.length(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float length(this float3 v) => math.length(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float lengthsq(this float2 v) => math.lengthsq(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float lengthsq(this float3 v) => math.lengthsq(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 cross(this float3 l, float3 r) => math.cross(l, r);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float dot(this float3 l, float3 r) => math.dot(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float2 reflect(this float2 i, float2 n) => math.reflect(i, n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 reflect(this float3 i, float3 n) => math.reflect(i, n);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 position(this float4x4 matrix) => new float3(matrix.c3.x, matrix.c3.y, matrix.c3.z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static quaternion toRotation(this float4x4 matrix)
    {
        float3 col0 = math.normalize(matrix.c0.xyz);
        float3 col1 = math.normalize(matrix.c1.xyz);
        float3 col2 = math.normalize(matrix.c2.xyz);

        float4x4 normalizedMatrix = new float4x4(new float4(col0, 0.0f),
                                                 new float4(col1, 0.0f),
                                                 new float4(col2, 0.0f),
                                                 matrix.c3);

        return quaternion.LookRotationSafe(normalizedMatrix.c2.xyz, normalizedMatrix.c1.xyz);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float3 scale(this float4x4 matrix) => new float3(math.length(matrix.c0.xyz), math.length(matrix.c1.xyz), math.length(matrix.c2.xyz));
}
}
