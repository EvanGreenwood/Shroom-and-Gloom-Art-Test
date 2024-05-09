#region
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
#endregion

namespace MathBad
{
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Int3
{
  public int x;
  public int y;
  public int z;

  [MethodImpl(256)]
  public Int3(int xyz)
  {
    x = y = z = xyz;
  }

  [MethodImpl(256)]
  public Int3(int x, int y, int z)
  {
    this.x = x;
    this.y = y;
    this.z = z;
  }

  [MethodImpl(256)] public static implicit operator Int3(int3 v) => new Int3(v.x, v.y, v.z);
  [MethodImpl(256)] public static implicit operator Int3(Vector3Int v) => new Int3(v.x, v.y, v.z);
  [MethodImpl(256)] public static implicit operator Int3(float3 v) => new Int3((int)v.x, (int)v.y, (int)v.z);
  [MethodImpl(256)] public static implicit operator Int3(Vector3 v) => new Int3((int)v.x, (int)v.y, (int)v.z);

  [MethodImpl(256)] public static explicit operator Int3(Int2 v) => new Int3(v.x, v.y, 0);
  [MethodImpl(256)] public static explicit operator Int3(int2 v) => new Int3(v.x, v.y, 0);
  [MethodImpl(256)] public static explicit operator Int3(Vector2Int v) => new Int3(v.x, v.y, 0);

  [MethodImpl(256)] public override string ToString() => $"({x}, {y}, {z})";
}
}
