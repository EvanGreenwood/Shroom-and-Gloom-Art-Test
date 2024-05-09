//  __  __  ___  ___  _  _                                                                            
// |  \/  || __|/ __|| || |                                                                           
// | |\/| || _| \__ \| __ |                                                                           
// |_|  |_||___||___/|_||_|                                                                           
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using UnityEngine;
#endregion

namespace MathBad
{
public static class MESH
{
  static MESH()
  {
    quadXZ = Resources.Load<Mesh>("Framework/Mesh/QuadXZ");
    quadXY_32X32 = Resources.Load<Mesh>("Framework/Mesh/QuadXY_32x32");
    quadXY_64X64 = Resources.Load<Mesh>("Framework/Mesh/QuadXY_64x64");
  }
  public static readonly Mesh quadXZ;
  public static readonly Mesh quadXY_32X32, quadXY_64X64;
}
}
