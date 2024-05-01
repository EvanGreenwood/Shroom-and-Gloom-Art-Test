#region
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
#endregion

namespace Mainframe
{
public static partial class MathExt
{
  public static Int2 Clamp(this Int2 v, int min, int max) => new Int2(v.x.Clamp(min, max), v.y.Clamp(min, max));
  public static Int2 Clamp(this Int2 v, Int2 min, Int2 max) => new Int2(v.x.Clamp(min.x, max.x), v.y.Clamp(min.y, max.y));
}

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Int2 : IEquatable<Int2>
{
  public int x;
  public int y;

  [MethodImpl(256)]
  public Int2(int xy)
  {
    x = xy;
    y = xy;
  }

  [MethodImpl(256)]
  public Int2(int x, int y)
  {
    this.x = x;
    this.y = y;
  }

  public static Int2 zero
  {
    [MethodImpl(256)]
    get => new Int2(0, 0);
  }

  // Implicit Conversion
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(256)] public static implicit operator Int2(int2 v) => new Int2(v.x, v.y);
  [MethodImpl(256)] public static implicit operator Int2(float2 v) => new Int2((int)v.x, (int)v.y);
  [MethodImpl(256)] public static implicit operator Int2(Vector2 v) => new Int2((int)v.x, (int)v.y);
  [MethodImpl(256)] public static implicit operator Int2(Vector2Int v) => new Int2(v.x, v.y);

  [MethodImpl(256)] public static explicit operator Vector2(Int2 v) => new Vector2((int)v.x, (int)v.y);

  // Operator Overloads
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(256)] public static Int2 operator +(Int2 a) => a;
  [MethodImpl(256)] public static Int2 operator -(Int2 a) => a;

  [MethodImpl(256)] public static Int2 operator +(Int2 a, int b) => new Int2(a.x + b, a.y + b);
  [MethodImpl(256)] public static Int2 operator -(Int2 a, int b) => new Int2(a.x - b, a.y - b);
  [MethodImpl(256)] public static Int2 operator *(Int2 a, int b) => new Int2(a.x * b, a.y * b);
  [MethodImpl(256)] public static Int2 operator /(Int2 a, int b) => new Int2(a.x / b, a.y / b);
  [MethodImpl(256)] public static Int2 operator +(int a, Int2 b) => new Int2(a + b.x, a + b.y);
  [MethodImpl(256)] public static Int2 operator -(int a, Int2 b) => new Int2(a - b.x, a - b.y);
  [MethodImpl(256)] public static Int2 operator *(int a, Int2 b) => new Int2(a * b.x, a * b.y);
  [MethodImpl(256)] public static Int2 operator /(int a, Int2 b) => new Int2(a / b.x, a / b.y);

  [MethodImpl(256)] public static Int2 operator +(Int2 a, float b) => new Int2(a.x + (int)b, a.y + (int)b);
  [MethodImpl(256)] public static Int2 operator -(Int2 a, float b) => new Int2(a.x - (int)b, a.y - (int)b);
  [MethodImpl(256)] public static Int2 operator *(Int2 a, float b) => new Int2(a.x * (int)b, a.y * (int)b);
  [MethodImpl(256)] public static Int2 operator /(Int2 a, float b) => new Int2(a.x / (int)b, a.y / (int)b);
  [MethodImpl(256)] public static Int2 operator +(float a, Int2 b) => new Int2((int)a + b.x, (int)a + b.y);
  [MethodImpl(256)] public static Int2 operator -(float a, Int2 b) => new Int2((int)a - b.x, (int)a - b.y);
  [MethodImpl(256)] public static Int2 operator *(float a, Int2 b) => new Int2((int)a * b.x, (int)a * b.y);
  [MethodImpl(256)] public static Int2 operator /(float a, Int2 b) => new Int2((int)a / b.x, (int)a / b.y);

  [MethodImpl(256)] public static Int2 operator +(Int2 a, Int2 b) => new Int2(a.x + b.x, a.y + b.y);
  [MethodImpl(256)] public static Int2 operator -(Int2 a, Int2 b) => new Int2(a.x - b.x, a.y - b.y);
  [MethodImpl(256)] public static Int2 operator *(Int2 a, Int2 b) => new Int2(a.x * b.x, a.y * b.y);
  [MethodImpl(256)] public static Int2 operator /(Int2 a, Int2 b) => new Int2(a.x / b.x, a.y / b.y);

  [MethodImpl(256)] public static Int2 operator +(Int2 a, Int3 b) => new Int2(a.x + b.x, a.y + b.y);
  [MethodImpl(256)] public static Int2 operator -(Int2 a, Int3 b) => new Int2(a.x - b.x, a.y - b.y);
  [MethodImpl(256)] public static Int2 operator *(Int2 a, Int3 b) => new Int2(a.x * b.x, a.y * b.y);
  [MethodImpl(256)] public static Int2 operator /(Int2 a, Int3 b) => new Int2(a.x / b.x, a.y / b.y);

  [MethodImpl(256)] public static Vector2 operator +(Int2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
  [MethodImpl(256)] public static Vector2 operator -(Int2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
  [MethodImpl(256)] public static Vector2 operator *(Int2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);
  [MethodImpl(256)] public static Vector2 operator /(Int2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);
  [MethodImpl(256)] public static Vector2 operator +(Vector2 a, Int2 b) => new Vector2(a.x + b.x, a.y + b.y);
  [MethodImpl(256)] public static Vector2 operator -(Vector2 a, Int2 b) => new Vector2(a.x - b.x, a.y - b.y);
  [MethodImpl(256)] public static Vector2 operator *(Vector2 a, Int2 b) => new Vector2(a.x * b.x, a.y * b.y);
  [MethodImpl(256)] public static Vector2 operator /(Vector2 a, Int2 b) => new Vector2(a.x / b.x, a.y / b.y);

  [MethodImpl(256)] public static Vector3 operator +(Int2 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, b.z);
  [MethodImpl(256)] public static Vector3 operator -(Int2 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, b.z);
  [MethodImpl(256)] public static Vector3 operator *(Int2 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, b.z);
  [MethodImpl(256)] public static Vector3 operator /(Int2 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.y, b.z);
  [MethodImpl(256)] public static Int2 operator +(Int2 a, Vector2Int b) => new Int2(a.x + b.x, a.y + b.y);
  [MethodImpl(256)] public static Int2 operator -(Int2 a, Vector2Int b) => new Int2(a.x - b.x, a.y - b.y);
  [MethodImpl(256)] public static Int2 operator *(Int2 a, Vector2Int b) => new Int2(a.x * b.x, a.y * b.y);
  [MethodImpl(256)] public static Int2 operator /(Int2 a, Vector2Int b) => new Int2(a.x / b.x, a.y / b.y);
  [MethodImpl(256)] public static Int2 operator +(Int2 a, int2 b) => new Int2(a.x + b.x, a.y + b.y);
  [MethodImpl(256)] public static Int2 operator -(Int2 a, int2 b) => new Int2(a.x - b.x, a.y - b.y);
  [MethodImpl(256)] public static Int2 operator *(Int2 a, int2 b) => new Int2(a.x * b.x, a.y * b.y);
  [MethodImpl(256)] public static Int2 operator /(Int2 a, int2 b) => new Int2(a.x / b.x, a.y / b.y);
  [MethodImpl(256)] public static Int2 operator +(Int2 a, int3 b) => new Int2(a.x + b.x, a.y + b.y);
  [MethodImpl(256)] public static Int2 operator -(Int2 a, int3 b) => new Int2(a.x - b.x, a.y - b.y);
  [MethodImpl(256)] public static Int2 operator *(Int2 a, int3 b) => new Int2(a.x * b.x, a.y * b.y);
  [MethodImpl(256)] public static Int2 operator /(Int2 a, int3 b) => new Int2(a.x / b.x, a.y / b.y);

  // Formatting
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(256)] public override string ToString() => $"({x}, {y})";

  // Equatable
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(256)] public bool Equals(Int2 other) => x == other.x && y == other.y;
  [MethodImpl(256)] public override bool Equals(object obj) => obj is Int2 other && Equals(other);
  [MethodImpl(256)] public override int GetHashCode() => HashCode.Combine(x, y);
  [MethodImpl(256)] public static bool operator ==(Int2 left, Int2 right) => left.Equals(right);
  [MethodImpl(256)] public static bool operator !=(Int2 left, Int2 right) => !left.Equals(right);
}
}
