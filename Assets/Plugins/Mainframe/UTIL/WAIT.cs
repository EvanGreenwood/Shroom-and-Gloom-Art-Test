// __      __   _    ___  _____                                                                       
// \ \    / /  /_\  |_ _||_   _|                                                                      
//  \ \/\/ /  / _ \  | |   | |                                                                        
//   \_/\_/  /_/ \_\|___|  |_|                                                                        
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
#endregion

namespace Mainframe
{
public static class WAIT
{
  [MethodImpl(256)]
  public static WaitForSeconds ForSeconds(float seconds) => new WaitForSeconds(seconds);
  [MethodImpl(256)]
  public static WaitForSecondsRealtime ForSecondsRealTime(float seconds) => new WaitForSecondsRealtime(seconds);

  [MethodImpl(256)]
  public static Coroutine WaitFrames(MonoBehaviour component, int frames, Action callback)
  {
    frames = math.max(0, frames);

    return component.StartCoroutine(WaitFramesCounter(frames, callback));
  }

  static IEnumerator WaitFramesCounter(int waitFrames, Action callback)
  {
    for(int i = 0; i < waitFrames; i++) yield return null;

    callback?.Invoke();
  }

  /// <summary> Wait for the end of the frame </summary>
  [MethodImpl(256)]
  public static WaitForEndOfFrame ForEndOfFrame() => new WaitForEndOfFrame();

  /// <summary> Wait for fixed update </summary>
  [MethodImpl(256)]
  public static WaitForFixedUpdate ForFixedUpdate() => new WaitForFixedUpdate();

  /// <summary> Wait for x number of frames </summary>
  [MethodImpl(256)]
  public static IEnumerator ForFrames(int x)
  {
    while(x > 0)
    {
      x--;
      yield return null;
    }
  }
}
}
