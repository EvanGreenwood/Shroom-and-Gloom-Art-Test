#region
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
#endregion

namespace MathBad
{
public static unsafe class CastingExt
{
    // Type Casting
    //----------------------------------------------------------------------------------------------------
    // Integral
    //----------------------------------------------------------------------------------------------------
    // sbyte    Signed 8-bit integer, range from -128 to 127.
    // byte     Unsigned 8-bit integer, range from 0 to 255.
    // short    Signed 16-bit integer, range from -32,768 to 32,767.
    // ushort   Unsigned 16-bit integer, range from 0 to 65,535.
    // int      Signed 32-bit integer, range from -2,147,483,648 to 2,147,483,647.
    // uint     Unsigned 32-bit integer, range from 0 to 4,294,967,295.
    // long     Signed 64-bit integer, range from -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.
    // ulong    Unsigned 64-bit integer, range from 0 to 18,446,744,073,709,551,615.
    // char     Represents a single 16-bit Unicode character, range from U+0000 to U+FFFF.

    // Floating-Point
    //----------------------------------------------------------------------------------------------------
    // float    Single-precision 32-bit IEEE 754 floating point. It has a precision of approximately 6-9 digits.
    // double   Double-precision 64-bit IEEE 754 floating point. It has a precision of approximately 15-17 digits.
    // decimal  128-bit precise decimal values with 28-29 significant digits.
    //----------------------------------------------------------------------------------------------------

    // byte
    [MethodImpl(256)] public static byte _byte(this int x) => (byte)x;
    [MethodImpl(256)] public static byte _byte(this uint x) => (byte)x;
    [MethodImpl(256)] public static byte _byte(this float x) => (byte)x;
    [MethodImpl(256)] public static byte _byte(this double x) => (byte)x;

    // int
    [MethodImpl(256)] public static int _int(this byte x) => (int)x;
    [MethodImpl(256)] public static int _int(this uint x) => *(int*)&x;
    [MethodImpl(256)] public static int _int(this float x) => (int)x;
    [MethodImpl(256)] public static int _int(this double x) => (int)x;

    // uint
    [MethodImpl(256)] public static uint _uint(this byte x) => (uint)x;
    [MethodImpl(256)] public static uint _uint(this int x) => *(uint*)&x;
    [MethodImpl(256)] public static uint _uint(this float x) => (uint)x;
    [MethodImpl(256)] public static uint _uint(this double x) => (uint)x;

    // float
    [MethodImpl(256)] public static float _float(this byte x) => (float)x;
    [MethodImpl(256)] public static float _float(this int x) => (float)x;
    [MethodImpl(256)] public static float _float(this uint x) => (float)x;
    [MethodImpl(256)] public static float _float(this double x) => (float)x;

    // double
    [MethodImpl(256)] public static double _double(this byte x) => (double)x;
    [MethodImpl(256)] public static double _double(this int x) => (double)x;
    [MethodImpl(256)] public static double _double(this uint x) => (double)x;
    [MethodImpl(256)] public static double _double(this float x) => (double)x;

    //----------------------------------------------------------------------------------------------------

    // Vector2
    [MethodImpl(256)] public static Vector2 _Vec2(this int xy) => new Vector2(xy, xy);
    [MethodImpl(256)] public static Vector2 _Vec2(this float xy) => new Vector2(xy, xy);

    [MethodImpl(256)] public static Vector2 _Vec2(this Vector3 v) => new Vector2(v.x, v.y);
    [MethodImpl(256)] public static Vector2 _Vec2(this float2 v) => *(Vector2*)&v;
    [MethodImpl(256)] public static Vector2 _Vec2(this float3 v) => new Vector2(v.x, v.y);

    [MethodImpl(256)] public static Vector2 _Vec2(this Vector2Int v) => new Vector2(v.x, v.y);
    [MethodImpl(256)] public static Vector2 _Vec2(this Vector3Int v) => new Vector2(v.x, v.y);
    [MethodImpl(256)] public static Vector2 _Vec2(this int2 v) => new Vector2(v.x, v.y);
    [MethodImpl(256)] public static Vector2 _Vec2(this int3 v) => new Vector2(v.x, v.y);

    // Vector3
    [MethodImpl(256)] public static Vector3 _Vec3(this int xyz) => new Vector3(xyz, xyz, xyz);
    [MethodImpl(256)] public static Vector3 _Vec3(this float xyz) => new Vector3(xyz, xyz, xyz);

    [MethodImpl(256)] public static Vector3 _Vec3(this Vector2 v, float z = 0.0f) => new Vector3(v.x, v.y, z);
    [MethodImpl(256)] public static Vector3 _Vec3XZ(this Vector2 v) => new Vector3(v.x, 0.0f, v.y);
    [MethodImpl(256)] public static Vector3 _Vec3(this float2 v, float z = 0.0f) => new Vector3(v.x, v.y, z);
    [MethodImpl(256)] public static Vector3 _Vec3(this float3 v) => *(Vector3*)&v;

    [MethodImpl(256)] public static Vector3 _Vec3(this Vector2Int v, float z = 0.0f) => new Vector3(v.x, v.y, z);
    [MethodImpl(256)] public static Vector3 _Vec3(this Vector3Int v) => new Vector3(v.x, v.y, v.z);
    [MethodImpl(256)] public static Vector3 _Vec3(this int2 v, float z = 0.0f) => new Vector3(v.x, v.y, z);
    [MethodImpl(256)] public static Vector3 _Vec3(this int3 v) => new Vector3(v.x, v.y, v.z);

    // Vector2Int
    [MethodImpl(256)] public static Vector2Int _Vec2i(this int xy) => new Vector2Int(xy, xy);
    [MethodImpl(256)] public static Vector2Int _Vec2i(this float xy) => new Vector2Int((int)xy, (int)xy);

    [MethodImpl(256)] public static Vector2Int _Vec2i(this Vector2 v) => new Vector2Int((int)v.x, (int)v.y);
    [MethodImpl(256)] public static Vector2Int _Vec2i(this Vector3 v) => new Vector2Int((int)v.x, (int)v.y);
    [MethodImpl(256)] public static Vector2Int _Vec2i(this float2 v) => new Vector2Int((int)v.x, (int)v.y);
    [MethodImpl(256)] public static Vector2Int _Vec2i(this float3 v) => new Vector2Int((int)v.x, (int)v.y);

    [MethodImpl(256)] public static Vector2Int _Vec2i(this Vector3Int v) => new Vector2Int(v.x, v.y);
    [MethodImpl(256)] public static Vector2Int _Vec2i(this int2 v) => *(Vector2Int*)&v;
    [MethodImpl(256)] public static Vector2Int _Vec2i(this int3 v) => new Vector2Int(v.x, v.y);

    // Vector3Int
    [MethodImpl(256)] public static Vector3Int _Vec3i(this int xyz) => new Vector3Int(xyz, xyz, xyz);
    [MethodImpl(256)] public static Vector3Int _Vec3i(this float xyz) => new Vector3Int((int)xyz, (int)xyz, (int)xyz);

    [MethodImpl(256)] public static Vector3Int _Vec3i(this Vector2 v, int z = 0) => new Vector3Int((int)v.x, (int)v.y, z);
    [MethodImpl(256)] public static Vector3Int _Vec3i(this Vector3 v) => new Vector3Int((int)v.x, (int)v.y, (int)v.z);
    [MethodImpl(256)] public static Vector3Int _Vec3i(this float2 v, int z = 0) => new Vector3Int((int)v.x, (int)v.y, z);
    [MethodImpl(256)] public static Vector3Int _Vec3i(this float3 v) => new Vector3Int((int)v.x, (int)v.y, (int)v.z);

    [MethodImpl(256)] public static Vector3Int _Vec3i(this Vector2Int v, int z = 0) => new Vector3Int((int)v.x, (int)v.y, z);
    [MethodImpl(256)] public static Vector3Int _Vec3i(this int2 v, int z = 0) => new Vector3Int((int)v.x, (int)v.y, z);
    [MethodImpl(256)] public static Vector3Int _Vec3i(this int3 v) => *(Vector3Int*)&v;

    // float2
    [MethodImpl(256)] public static float2 _float2(this int xy) => new float2(xy, xy);
    [MethodImpl(256)] public static float2 _float2(this float xy) => new float2(xy, xy);

    [MethodImpl(256)] public static float2 _float2(this Vector2 v) => *(float2*)&v;
    [MethodImpl(256)] public static float2 _float2(this Vector3 v) => new float2(v.x, v.y);
    [MethodImpl(256)] public static float2 _float2(this float3 v) => new float2(v.x, v.y);

    [MethodImpl(256)] public static float2 _float2(this Vector2Int v) => new float2(v.x, v.y);
    [MethodImpl(256)] public static float2 _float2(this Vector3Int v) => new float2(v.x, v.y);
    [MethodImpl(256)] public static float2 _float2(this int2 v) => new float2(v.x, v.y);
    [MethodImpl(256)] public static float2 _float2(this int3 v) => new float2(v.x, v.y);

    // float3
    [MethodImpl(256)] public static float3 _float3(this int xyz) => new float3(xyz, xyz, xyz);
    [MethodImpl(256)] public static float3 _float3(this float xyz) => new float3(xyz, xyz, xyz);

    [MethodImpl(256)] public static float3 _float3(this Vector2 v, float z = 0.0f) => new float3(v.x, v.y, z);
    [MethodImpl(256)] public static float3 _float3(this Vector3 v) => *(float3*)&v;
    [MethodImpl(256)] public static float3 _float3(this float2 v, float z = 0.0f) => new float3(v.x, v.y, z);
    [MethodImpl(256)] public static float3 _float3XZ(this float2 v) => new float3(v.x, 0f, v.y);

    [MethodImpl(256)] public static float3 _float3(this Vector2Int v, float z = 0.0f) => new float3(v.x, v.y, z);
    [MethodImpl(256)] public static float3 _float3(this Vector3Int v) => new float3(v.x, v.y, v.z);
    [MethodImpl(256)] public static float3 _float3(this int2 v, float z = 0.0f) => new float3(v.x, v.y, z);
    [MethodImpl(256)] public static float3 _float3(this int3 v) => new float3(v.x, v.y, v.z);

    // int2
    [MethodImpl(256)] public static int2 _int2(this int xy) => new int2(xy, xy);
    [MethodImpl(256)] public static int2 _int2(this float xy) => new int2((int)xy, (int)xy);

    [MethodImpl(256)] public static int2 _int2(this float2 v) => new int2((int)v.x, (int)v.y);
    [MethodImpl(256)] public static int2 _int2(this float3 v) => new int2((int)v.x, (int)v.y);
    [MethodImpl(256)] public static int2 _int2(this Vector2 v) => new int2((int)v.x, (int)v.y);
    [MethodImpl(256)] public static int2 _int2(this Vector3 v) => new int2((int)v.x, (int)v.y);

    [MethodImpl(256)] public static int2 _int2(this Vector2Int v) => *(int2*)&v;
    [MethodImpl(256)] public static int2 _int2(this Vector3Int v) => new int2(v.x, v.y);
    [MethodImpl(256)] public static int2 _int2(this int3 v) => new int2(v.x, v.y);

    // int3
    [MethodImpl(256)] public static int3 _int3(this int xyz) => new int3(xyz, xyz, xyz);
    [MethodImpl(256)] public static int3 _int3(this float xyz) => new int3((int)xyz, (int)xyz, (int)xyz);

    [MethodImpl(256)] public static int3 _int3(this float2 v, int z = 0) => new int3((int)v.x, (int)v.y, z);
    [MethodImpl(256)] public static int3 _int3(this float3 v) => new int3((int)v.x, (int)v.y, (int)v.z);
    [MethodImpl(256)] public static int3 _int3(this Vector2 v, int z = 0) => new int3((int)v.x, (int)v.y, z);
    [MethodImpl(256)] public static int3 _int3(this Vector3 v) => new int3((int)v.x, (int)v.y, (int)v.z);

    [MethodImpl(256)] public static int3 _int3(this Vector2Int v, int z = 0) => new int3(v.x, v.y, z);
    [MethodImpl(256)] public static int3 _int3(this Vector3Int v) => *(int3*)&v;
    [MethodImpl(256)] public static int3 _int3(this int2 v, int z = 0) => new int3(v.x, v.y, z);

    // quaternion
    [MethodImpl(256)] public static quaternion _quat(this Quaternion q) => *(quaternion*)&q;

    // Quaternion
    [MethodImpl(256)] public static Quaternion _Quat(this quaternion q) => *(Quaternion*)&q.value;
}
}
