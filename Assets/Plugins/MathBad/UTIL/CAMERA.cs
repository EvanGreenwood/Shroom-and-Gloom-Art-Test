//   ___    _    __  __  ___  ___    _                                                                
//  / __|  /_\  |  \/  || __|| _ \  /_\                                                               
// | (__  / _ \ | |\/| || _| |   / / _ \                                                              
//  \___|/_/ \_\|_|  |_||___||_|_\/_/ \_\                                                             
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using UnityEngine;
#endregion

namespace MathBad
{
public enum CameraID
{
  MainCamera = 0,
  UICamera = 1,
}

public static class CAMERA
{
  static Camera _mainCameraInternal;
  static Camera _uiCameraInternal;

  public static Camera main
  {
    get
    {
      if(!_mainCameraInternal)
        _mainCameraInternal = Camera.main;
      return _mainCameraInternal;
    }
  }
  
  public static Camera ui
  {
    get
    {
      if(!_uiCameraInternal)
      {
        GameObject gameObject = GameObject.Find("UICamera");
        if(gameObject)
        {
          _uiCameraInternal = gameObject.GetComponent<Camera>();
        }
      }
      return _uiCameraInternal;
    }
  }
  
  public static Camera ByID(CameraID id)
  {
    return id switch
           {
             CameraID.MainCamera => main,
             CameraID.UICamera   => ui,
             _                   => null
           };
  }
  
  public static Vector2 mouseWorld => main.ScreenToWorldPoint(INPUT.mousePos);
  public static Vector2 mouseWorldUI => ui.ScreenToWorldPoint(INPUT.mousePos);
}
}
