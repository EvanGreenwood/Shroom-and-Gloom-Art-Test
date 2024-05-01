//  _      ___     _    ___                                                                           
// | |    / _ \   /_\  |   \                                                                          
// | |__ | (_) | / _ \ | |) |                                                                         
// |____| \___/ /_/ \_\|___/                                                                          
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using UnityEngine;
#endregion

namespace Mainframe
{
public class LOAD
{
  public static T IfNull<T>(string path, ref T result) where T : Object
  {
    if(result == null)
    {
      result = Resources.Load<T>(path);
      if(result == null)
      {
        Debug.Log("Resources: ".Red() + $"Failed to load asset at {path}.");
      }
    }
    return result;
  }
}
}
