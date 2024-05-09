#region
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
#endregion
namespace MathBad
{
public static class ShaderExt
{
  [MethodImpl(256)]
  public static void Dispatch(this ComputeShader shader, int kernel, uint3 threadGroupSizes)
  {
    shader.Dispatch(kernel, (int)threadGroupSizes.x, (int)threadGroupSizes.y, (int)threadGroupSizes.z);
  }
}
}
