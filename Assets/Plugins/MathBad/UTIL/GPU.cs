//   ___  ___  _   _                                                                                  
//  / __|| _ \| | | |                                                                                 
// | (_ ||  _/| |_| |                                                                                 
//  \___||_|   \___/                                                                                  
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
#endregion

namespace MathBad
{
public class GPU
{
  public static int SizeOf<T>() where T : unmanaged => Marshal.SizeOf<T>();

  public static uint3 GetKernelThreadGroupSizes(ComputeShader shader, int kernel)
  {
    shader.GetKernelThreadGroupSizes(kernel, out uint x, out uint y, out uint z);
    return new uint3(x, y, z);
  }
}
}
