//  ___  _  _   ___                                                                                   
// | _ \| \| | / __|                                                                                  
// |   /| .` || (_ |                                                                                  
// |_|_\|_|\_| \___|                                                                                  
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
#endregion

namespace MathBad
{
public static class RNG
{
  static Random _rng;

  static RNG()
  {
    uint seed = (uint)DateTime.Now.Ticks;
    _rng = new Random(seed);
  }

  [MethodImpl(256)] public static float Float() => _rng.NextFloat();
  [MethodImpl(256)] public static float Float(float max) => _rng.NextFloat(max);
  [MethodImpl(256)] public static float Float(float min, float max) => _rng.NextFloat(min, max);
  [MethodImpl(256)] public static float FloatSign() => Float() * 2f - 1f;
  [MethodImpl(256)] public static float FloatSign(float max) => Float(max) * 2f - 1f;
  [MethodImpl(256)] public static float FloatVariance(float x, float variance) => Float(x - variance * 0.5f, x + variance * 0.5f);

  [MethodImpl(256)] public static int Int() => _rng.NextInt();
  [MethodImpl(256)] public static int Int(int max) => _rng.NextInt(max);
  [MethodImpl(256)] public static int Int(int min, int max) => _rng.NextInt(min, max);

  [MethodImpl(256)] public static Vector2 Vector2() => _rng.NextFloat2();
  [MethodImpl(256)] public static Vector2 Vector2(Vector2 max) => _rng.NextFloat2(max);
  [MethodImpl(256)] public static Vector2 Vector2(Vector2 min, Vector2 max) => _rng.NextFloat2(min, max);
  [MethodImpl(256)] public static Vector2 Vector2InsideUnitCircle() => new Vector2(FloatSign(), FloatSign()).normalized * Float();
  [MethodImpl(256)] public static Vector2 Vector2Direction() => _rng.NextFloat2Direction();

  [MethodImpl(256)] public static Vector3 Vector3() => _rng.NextFloat3();
  [MethodImpl(256)] public static Vector3 Vector3(Vector3 max) => _rng.NextFloat3(max);
  [MethodImpl(256)] public static Vector3 Vector3(Vector3 min, Vector3 max) => _rng.NextFloat3(min, max);
  [MethodImpl(256)] public static Vector3 Vector3Direction() => _rng.NextFloat3Direction();

  [MethodImpl(256)] public static Vector4 Vector4() => _rng.NextFloat4();
  [MethodImpl(256)] public static Vector4 Vector4(Vector4 max) => _rng.NextFloat4(max);
  [MethodImpl(256)] public static Vector4 Vector4(Vector4 min, Vector4 max) => _rng.NextFloat4(min, max);

  [MethodImpl(256)] public static float2 Float2() => _rng.NextFloat2();
  [MethodImpl(256)] public static float2 Float2(float2 max) => _rng.NextFloat2(max);
  [MethodImpl(256)] public static float2 Float2(float2 min, float2 max) => _rng.NextFloat2(min, max);
  [MethodImpl(256)] public static float2 Float2Direction() => _rng.NextFloat2Direction();

  [MethodImpl(256)] public static float3 Float3() => _rng.NextFloat3();
  [MethodImpl(256)] public static float3 Float3(float3 max) => _rng.NextFloat3(max);
  [MethodImpl(256)] public static float3 Float3(float3 min, float3 max) => _rng.NextFloat3(min, max);
  [MethodImpl(256)] public static float3 Float3Direction() => _rng.NextFloat3Direction();

  [MethodImpl(256)] public static Quaternion RotationZ() => Quaternion.Euler(0f, 0f, Float(360f));

  [MethodImpl(256)] public static bool Probability(float probability) => Float() <= probability;
  [MethodImpl(256)] public static bool CoinFlip() => Probability(0.5f);
}
}
